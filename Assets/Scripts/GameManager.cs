using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    IEnumerator Start()
    {
        isSetUpEnd = false;

        // ゲームクリアセットの初期設定
        canvasGroupGameClear.alpha = 0;
        imgGameClear.transform.localScale = Vector3.zero;

        // EnemyGeneratorの初期設定
        enemyGenerator.SetUpEnemyGenerator(playerController, this);

        // バレットのボタンを生成
        yield return StartCoroutine(bulletSelectManager.GenerateBulletSelectDetail(playerController));

        // 使用できるバレットの確認と更新
        bulletSelectManager.JugdeOpenBullets();

        isSetUpEnd = true;

        StartCoroutine(ObservateGenerateEnemyState());
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
    }

    /// <summary>
    /// Wave進行
    /// </summary>
    private void AdvanceWave() {
        waveCount++;
        Debug.Log(waveCount);
        if (waveCount == maxWaveCount - 1) {
            Debug.Log("Boss");

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
                    });
            });
    }

    void Update()
    {
        
    }
}
