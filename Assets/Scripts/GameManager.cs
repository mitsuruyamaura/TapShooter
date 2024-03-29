using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private EnemyGenerator enemyGenerator;

    [SerializeField]
    private BulletSelectManager bulletSelectManager;

    [SerializeField]
    private PlayerController playerController;

    public bool isSetUpEnd;

    public bool isGameUp;

    public int waveCount = 0;

    public int maxWaveCount;

    [SerializeField]
    private CanvasGroup canvasGroupGameClear;

    [SerializeField]
    private Image imgGameClear;

    [SerializeField]
    private CanvasGroup canvasGroupOpeningFilter;

    [SerializeField]
    private Image imgGameStart;

    [SerializeField]
    private GameObject fireworksPrefab;

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private Button btnRestart;

    private bool isClickable;

    [SerializeField]
    private CanvasGroup canvasGroupRestartImage;

    [SerializeField]
    private Text txtTresureBoxCount;

    private int tresureBoxCount;

    [SerializeField]
    private GameObject charaObj;

    [SerializeField]
    private DefenseBase defenseBase;

    [SerializeField]
    private ChooseBulletPopUp chooseBulletPopUp;

    public bool isChooseBulletEnd;

    IEnumerator Start()
    {
        // 準備開始
        isSetUpEnd = false;

        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Main);

        // DefenseBase の設定を行う
        defenseBase.SetUpDefenseBase(this);

        // キャラの現在のサイズを保持し、一時、見えなくする
        float scaleX = charaObj.transform.localScale.x;
        charaObj.transform.localScale = Vector3.zero;

        // ゲームクリアセットの初期設定
        canvasGroupGameClear.alpha = 0;
        imgGameClear.transform.localScale = Vector3.zero;
        btnRestart.onClick.AddListener(() => OnClickRestart(canvasGroupGameClear));
        canvasGroupRestartImage.alpha = 0;

        // EnemyGeneratorの初期設定
        enemyGenerator.SetUpEnemyGenerator(playerController, this);

        // バレット選択ポップアップを利用するか、初期設定バレットですぐにゲーム開始する確認
        if (!GameData.instance.isDebugDefaultBulletCreate) {
            // 暗転を解除
            canvasGroupOpeningFilter.DOFade(0.0f, 1.0f);

            // バレット選択ポップアップを開き、選択できるバレットをアイテムバレットとして生成してゲームに使用するバレットの登録開始。登録終了するまでここで停止
            yield return StartCoroutine(OpenChooseBulletPopUp());
        }

        // バレット選択用のボタンを生成
        yield return StartCoroutine(bulletSelectManager.GenerateBulletSelectDetail(playerController));

        // 使用できるバレットの確認と更新
        //bulletSelectManager.JugdeOpenBullets();

        // ゲームスタート時の演出
        yield return StartCoroutine(Opening());

        // キャラ表示(スタート演出の途中でキャラをタイミングよく表示させる)
        Sequence sequence = DOTween.Sequence();

        sequence.Append(charaObj.transform.DOScale(Vector3.one * 1.3f, 0.5f).SetEase(Ease.Linear));
        sequence.Append(charaObj.transform.DOScale(Vector3.one * scaleX, 0.05f).SetEase(Ease.Linear));
        
        //charaObj.transform.DOScale(Vector3.one * scaleX, 1.0f).SetEase(Ease.Linear);

        //Debug.Log(charaObj.transform.localScale.x);

        // スタート演出が終了するまで一時処理を中断して待機(この間にキャラは元の大きさに戻っている)
        yield return new WaitForSeconds(1.0f);

        // 開始時のSE
        SoundManager.instance.SetStartVoice((SoundManager.StartVoice)Random.Range(0, SoundManager.instance.StartVoice_Clips.Length));

        yield return new WaitForSeconds(0.5f);

        // 使用できるバレットの確認と更新。初期バレットをアニメさせる
        bulletSelectManager.JugdeOpenBullets();

        // 準備完了状態にして、画面のタップを受け付ける
        isSetUpEnd = true;

        // エネミーの生成の監視スタート
        StartCoroutine(ObservateGenerateEnemyState());
    }

    /// <summary>
    /// バレット選択ポップアップの制御
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenChooseBulletPopUp() {

        // バレット選択ポップアップ表示
        chooseBulletPopUp.CreateChooseBulletDetails(this);

        // バレットの選択が終了するまで待機
        yield return new WaitUntil(() => isChooseBulletEnd);
    }

    /// <summary>
    /// 選択したバレットの登録と選択完了処理
    /// </summary>
    public void SetBulletDatas(List<BulletDataSO.BulletData> bulletDatas) {

        // GameData に選択したバレットデータを登録
        GameData.instance.SetChooseBulletDatas(bulletDatas);

        // バレットの選択完了して次の処理へ移る
        isChooseBulletEnd = true;
    }

    private IEnumerator Opening() {
        canvasGroupOpeningFilter.DOFade(0.0f, 1.0f)
            .OnComplete(() => 
            {
                imgGameStart.transform.DOLocalJump(Vector3.zero, 300.0f, 3, 1.5f).SetEase(Ease.Linear);
                //imgGameStart.transform.DOLocalMoveX(0, 1.0f);
            });

        yield return new WaitForSeconds(3.5f);
        imgGameStart.transform.DOLocalJump(new Vector3(1500, 0, 0), 200.0f, 3, 1.5f).SetEase(Ease.Linear);
        //imgGameStart.transform.DOLocalMoveX(1500, 1.0f);
    }

    /// <summary>
    /// 敵の生成を監視
    /// </summary>
    /// <returns></returns>
    private IEnumerator ObservateGenerateEnemyState() {
        Debug.Log("監視開始");
        bool isAllGenerate = false;

        while (!isAllGenerate) {
            if (enemyGenerator.isGenerateEnd) {
                isAllGenerate = true;
            }
            yield return null;
        }
        Debug.Log("監視終了");

        if (waveCount >= maxWaveCount) {
            Debug.Log("Game Clear");
        } else {
            AdvanceWave();
        }   
    }

    private IEnumerator ObservateBossState() {
        Debug.Log("ボス監視開始");

        while (!enemyGenerator.isBossDestroyed) {
            yield return null;
        }

        Debug.Log("ゲームクリア");

        // ゲームクリア判定更新
        SwitchGameUp(true);

        // クリア表示
        DisplayGameClear();

        // すべてのバレットを押せない状態にする
        bulletSelectManager.InactivateAllBulletBtns();

        // クリア演出
        StartCoroutine(GenerateFireWorks());

        yield return new WaitForSeconds(7.5f);

        // クリアボイス再生
        SoundManager.instance.SetGameUpVoice(SoundManager.GameUpVoice.Win_1, 0.5f);
    }

    /// <summary>
    /// Wave進行
    /// </summary>
    private void AdvanceWave() {
        waveCount++;
        Debug.Log(waveCount);
        if (waveCount == maxWaveCount - 1) {
            Debug.Log("Boss");

            // ボスのBGM再生
            SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Boss);

            // ボス前の警告ボイス再生
            SoundManager.instance.SetWarningVoice((SoundManager.WarningVoice)Random.Range(0, SoundManager.instance.WarningVoice_Clips.Length));

            // ボス生成
            StartCoroutine(enemyGenerator.GenerateBoss());

            // ボス監視
            StartCoroutine(ObservateBossState());

        } else {
            // 生成再開
            enemyGenerator.SwitchGenerateState(false);

            // 監視再開
            StartCoroutine(ObservateGenerateEnemyState());
        }
    }

    /// <summary>
    /// ゲーム終了状態を切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchGameUp(bool isSwitch) {
        isGameUp = isSwitch;

        if (isGameUp) {
            // エネミーをすべて削除
            enemyGenerator.ClearEnemyList();

            // 敵のバレットをすべて削除
            enemyGenerator.DestroyTemporaryObjectContainer();
        }
    }

    /// <summary>
    /// ゲームクリア表示
    /// </summary>
    private void DisplayGameClear() {
        canvasGroupGameClear.DOFade(1.0f, 0.25f)
            .OnComplete(()=> 
            {
                imgGameClear.transform.DOPunchScale(Vector3.one * 2.5f, 0.5f)
                    .OnComplete(()=> 
                    {
                        imgGameClear.transform.DOShakeScale(0.5f);
                        imgGameClear.transform.localScale = Vector3.one * 1.5f;

                        // ゲームクリアBGM
                        SoundManager.instance.PlaySE(SoundManager.SE_Type.GameClear, 0.1f);

                        // 画面タップを許可
                        canvasGroupGameClear.blocksRaycasts = true;

                        // Restartの点滅表示
                        canvasGroupRestartImage.DOFade(1.0f, 1.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                    });
            });
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 花火の生成
    /// </summary>
    private IEnumerator GenerateFireWorks() {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < Random.Range(5, 8); i++) {
            GameObject fireworks = Instantiate(fireworksPrefab, canvasTran, false);
            
            // 色を変更         
            ParticleSystem.MainModule main = fireworks.GetComponent<ParticleSystem>().main;
            main.startColor = new ParticleSystem.MinMaxGradient(GetNewParticleColor());

            // 位置変更
            fireworks.transform.localPosition = new Vector3(fireworks.transform.localPosition.x + Random.Range(-500, 500), fireworks.transform.localPosition.y + Random.Range(700, 1000));
            
            Destroy(fireworks, 3f);
            
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// パーティクルの色をランダムで設定
    /// </summary>
    /// <returns></returns>
    private Color GetNewParticleColor() {
        int r = Random.Range(0, 255);
        int g = Random.Range(0, 255);
        int b = Random.Range(0, 255);
        Color newColor = new Color32((byte)r, (byte)g, (byte)b, 255);
        return newColor;         
    }

    /// <summary>
    /// ゲームクリア後のタップ処理
    /// </summary>
    public void OnClickRestart(CanvasGroup canvasGroup) {
        // クリック済なら処理しない
        if (isClickable) {
            return;
        }

        // 重複防止
        isClickable = true;

        // リスタート
        StartCoroutine(RestartGame(canvasGroup));
    }

    /// <summary>
    /// リスタート
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestartGame(CanvasGroup canvasGroup) {

        canvasGroup.DOFade(0, 1.5f);
        yield return new WaitForSeconds(1.5f);

        // イベントにイベントハンドラーを追加
        SceneManager.sceneLoaded += SceneLoaded;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// イベントハンドラー。シーン遷移時に自動的に実行される
    /// </summary>
    /// <param name="nextScene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene nextScene, LoadSceneMode mode) {
        Debug.Log("リロード時に呼ばれるよ");
        DataBaseManager.instance.Initialize();
    }

    /// <summary>
    /// 宝箱の獲得数を加算して表示更新
    /// </summary>
    public void UpdateTreasureBoxCount() {
        tresureBoxCount++;
        Debug.Log("現在の獲得宝箱数 : " + tresureBoxCount);

        txtTresureBoxCount.text = tresureBoxCount.ToString();
    }
}