using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Enemy : MonoBehaviour
{
    // 弾のクラス
    public Bullet bulletPrefab;

    private UnityAction<Transform> moveEvent;

    public EnemyDataSO.EnemyData enemyData;

    public PlayerController playerController;

    [SerializeField]
    private Image imgEnemy;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private CanvasGroup canvasGroupSlider;

    [SerializeField]
    private Transform floatingDamageTran;

    [SerializeField]
    private FloatingMessage floatingMessagePrefab;

    private int maxHp;

    private EnemyGenerator enemyGenerator;

    private int hp;

    [SerializeField]
    private GameObject bulletEffectPrefab;

    [SerializeField]
    private TreasureBox treasureBoxPrefab;

    [Header("宝箱の出現確率"), Range(0, 100)]
    public int appearTreasureRate;


    //void Start() {
        
    //    maxHp = enemyData.hp;
    //    hp = maxHp;

    //    UpDateDisplayHpGauge();
    //}

    //void Update() {
    //    transform.Translate(0, -0.01f, 0);
    //    if (transform.localPosition.y < -3000) {
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Bullet") {
            Debug.Log(col.gameObject);
            if (col.gameObject.TryGetComponent(out Bullet bullet)) {
                UpdateHp(bullet);
                GenerateBulletEffect(col.gameObject.transform);
            }
        }
    }

    /// <summary>
    /// HP更新
    /// </summary>
    /// <param name="bullet"></param>
    private void UpdateHp(Bullet bullet) {
        if(ElementCompatibilityHelper.GetElementCompatibility(bullet.bulletData.elementType, enemyData.elementType) == true) {
            hp -= bullet.bulletData.bulletPower * 2;
            Debug.Log("Element 相性　良");

            CreateFloatingMessageToDamage(bullet.bulletData.bulletPower * 2, true);
        } else {
            hp -= bullet.bulletData.bulletPower;
            CreateFloatingMessageToDamage(bullet.bulletData.bulletPower, false);
        }

        //hp -= bullet.bulletData.bulletPower;
        
        Debug.Log("Hp : " + hp);

        // TODO SE
        SoundManager.instance.PlaySE(SoundManager.SE_Type.BulletDamage_1);

        //CreateFloatingDamage(bullet.bulletData.bulletPower);
        UpDateDisplayHpGauge();

        if (hp <= 0) {
            hp = 0;

            if (enemyData.enemyType == EnemyDataSO.EnemyType.Boss) {
                enemyGenerator.isBossDestroyed = true;
            }

            // EXP加算
            enemyGenerator.UpdateExp(enemyData.exp);

            // 宝箱生成判定
            if (JudgeGenerateTrasureBox()) {
                
                // 宝箱生成
                GenerateTreasureBox();
            }

            Destroy(gameObject);
        }

        // ノーマル弾の場合
        if (bullet.bulletData.bulletType == BulletDataSO.BulletType.Player_Normal || bullet.bulletData.bulletType == BulletDataSO.BulletType.Player_5ways_Normal) {
            // 破壊
            Destroy(bullet.gameObject);
        }
    }

    /// <summary>
    /// HPゲージの表示更新
    /// </summary>
    private void UpDateDisplayHpGauge() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupSlider.DOFade(1.0f, 0.15f));
        sequence.Join(slider.DOValue((float)hp / maxHp, 0.25f));
        sequence.AppendInterval(0.25f);
        sequence.Append(canvasGroupSlider.DOFade(0f, 0.15f));
    }

    /// <summary>
    /// ダメージ表示の生成
    /// </summary>
    /// <param name="bulletPower"></param>
    private void CreateFloatingMessageToDamage(int bulletPower, bool isElementCompatibility) {
        FloatingMessage floatingMessage = Instantiate(floatingMessagePrefab, floatingDamageTran);
        floatingMessage.DisplayFloatingMessage(bulletPower, FloatingMessage.FloatingMessageType.EnemyDamage , isElementCompatibility);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="playerController"></param>
    /// <param name="bulletData"></param>
    /// <param name="enemyData"></param>
    /// <param name="bulletPrefab"></param>

    public void Inisialize(PlayerController playerController, BulletDataSO.BulletData bulletData, EnemyDataSO.EnemyData enemyData, Bullet bulletPrefab = null)
    {
        transform.localPosition = new Vector3(transform.localPosition.x + Random.Range(-650, 650), transform.localPosition.y, 0);

        this.playerController = playerController;

        this.enemyData = enemyData;
        imgEnemy.sprite = this.enemyData.enemySprite;

        maxHp = enemyData.hp;
        hp = maxHp;

        UpDateDisplayHpGauge();

        if (bulletPrefab != null) {

            this.bulletPrefab = bulletPrefab;

            // 弾の自動生成          
            StartCoroutine(EnemyShot(bulletData));
        }

        moveEvent = DataBaseManager.instance.moveDataSO.GetMoveEvent(enemyData.enemyType);

        moveEvent.Invoke(transform);
    }

    /// <summary>
    /// 弾の自動生成
    /// </summary>
    /// <param name="bulletData"></param>
    /// <returns></returns>
    private IEnumerator EnemyShot(BulletDataSO.BulletData bulletData) {
                
        while (true) {
            Bullet bullet = Instantiate(bulletPrefab, transform);
            bullet.ShotBullet(bulletData, GetPlayerDirection());
            yield return new WaitForSeconds(bulletData.loadingTime);
        }
    }

    /// <summary>
    /// プレイヤーとエネミーとの位置から方向を判定
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerDirection() {
        return (playerController.transform.position - transform.position).normalized;
    }

    /// <summary>
    /// 追加の初期設定
    /// </summary>
    /// <param name="enemyGenerator"></param>
    public void AdditionalInitialize(EnemyGenerator enemyGenerator) {
        this.enemyGenerator = enemyGenerator;
    }

    /// <summary>
    /// 被バレット時のヒット演出用のエフェクト生成
    /// </summary>
    /// <param name="tran"></param>
    private void GenerateBulletEffect(Transform tran) {
        GameObject effect = Instantiate(bulletEffectPrefab, tran, false);
        //effect.transform.SetParent(GameObject.FindGameObjectWithTag("BulletPool").transform);
        effect.transform.SetParent(DataBaseManager.instance.GetTemporaryObjectContainerTransform());
        Destroy(effect, 3.0f);
    }

    /// <summary>
    /// 宝箱が出現するか判定
    /// </summary>
    /// <returns></returns>
    private bool JudgeGenerateTrasureBox() {
        return Random.Range(0, 100) < appearTreasureRate ? true : false;
    }

    /// <summary>
    /// 宝箱生成
    /// </summary>
    private void GenerateTreasureBox() {
        TreasureBox treasureBox = Instantiate(treasureBoxPrefab, transform, false);
        treasureBox.transform.SetParent(DataBaseManager.instance.GetTemporaryObjectContainerTransform());
        treasureBox.SetUpTreasureBox(enemyGenerator);
    }
}
