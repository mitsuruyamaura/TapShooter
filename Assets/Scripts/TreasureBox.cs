using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System; //mi 155

public class TreasureBox : MonoBehaviour
{
    [SerializeField]
    private Button btnTresureBox;

    private bool isClickable;

    [SerializeField]
    private GameObject appearEffectPrefab;

    [SerializeField]
    private GameObject getEffectPrefab;

    private Vector3 startScale;

    private EnemyGenerator enemyGenerator;


    void Start() {
        //SetUpTreasureBox(GameObject.FindGameObjectWithTag("TreasureBox").transform);
    }

    /// <summary>
    /// 宝箱の初期設定
    /// </summary>
    public void SetUpTreasureBox(EnemyGenerator enemyGenerator) {
        // タップ防止
        isClickable = true;

        this.enemyGenerator = enemyGenerator;

        // 本来の宝箱のサイズを変数に保持しておく
        startScale = transform.localScale;

        // 宝箱のサイズを 0 にして見えない状態にする
        transform.localScale = Vector3.zero;

        // ボタンにメソッドを登録
        btnTresureBox.onClick.AddListener(OnClickTreasureBox);

        // 出現エフェクト生成
        CreateAppearEffect();

        // TODO 出現SE

    }

    /// <summary>
    /// 出現エフェクト生成
    /// </summary>
    private void CreateAppearEffect() {
        GameObject effect = Instantiate(appearEffectPrefab, transform, false);
        Destroy(effect, 3.0f);

        // 出現エフェクトに合わせて宝箱の大きさを徐々に戻す。それからタップ可能にする(出現エフェクトが消えてから)
        transform.DOScale(startScale, 2.5f).OnComplete(() => { isClickable = false; });
    }

    /// <summary>
    /// 宝箱をタップした際の処理
    /// </summary>
    private void OnClickTreasureBox() {
        if (isClickable) {
            return;
        }

        // 重複防止
        isClickable = true;

        // 宝箱獲得数の加算を通知
        enemyGenerator.NoticeTeasureBoxCountToGameManager();

        Sequence sequence = DOTween.Sequence();

        // 宝箱アイコンの位置情報を取得
        Transform targetTran = DataBaseManager.instance.GetTresureBoxIconTransfrom();

        // 画面左上の宝箱アイコンの位置まで、大きさを小さくしながら移動
        sequence.Append(transform.DOLocalMove(targetTran.localPosition, 1.5f));
        sequence.Join(transform.DOScale(Vector3.one * 0.25f, 1.5f))
            .OnComplete(() => 
            {
                // 宝箱アイコンの位置で獲得エフェクトを生成
                GameObject effect = Instantiate(getEffectPrefab, targetTran, false);
                Destroy(effect, 3.0f);

                // TODO 獲得SE

                Destroy(gameObject);
            });
    }
}
