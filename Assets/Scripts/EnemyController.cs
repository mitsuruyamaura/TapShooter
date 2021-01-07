using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyController : MonoBehaviour
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
    private FloatingMessage floatingDamagePrefab;

    private int maxHp;

    private EnemyGenerator enemyGenerator;

    private int hp;

    //void Start() {
        
    //    maxHp = enemyData.hp;
    //    hp = maxHp;

    //    UpDateDisplayHpGauge();
    //}

    void Update() {
        transform.Translate(0, -0.01f, 0);
        if (transform.localPosition.y < -3000) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Bullet") {
            Debug.Log(col.gameObject);
            if (col.gameObject.TryGetComponent(out Bullet bullet)) {
                UpdateHp(bullet);
            }
        }
    }

    /// <summary>
    /// HP更新
    /// </summary>
    /// <param name="bullet"></param>
    private void UpdateHp(Bullet bullet) {
        hp -= bullet.bulletData.bulletPower;
        Debug.Log("Hp : " + hp);

        CreateFloatingDamage(bullet.bulletData.bulletPower);
        UpDateDisplayHpGauge();

        if (hp <= 0) {
            hp = 0;

            if (enemyData.enemyType == EnemyDataSO.EnemyType.Boss) {
                enemyGenerator.isBossDestroyed = true;
            }

            // EXP加算
            enemyGenerator.UpdateExp(enemyData.exp);

            Destroy(gameObject);
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
    private void CreateFloatingDamage(int bulletPower) {
        FloatingMessage floatingDamage = Instantiate(floatingDamagePrefab, floatingDamageTran);
        floatingDamage.DisplayFloatingDamage(bulletPower);
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
            bullet.Shot(bulletData, GetPlayerDirection());
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

    public void AdditionalInitialize(EnemyGenerator enemyGenerator) {
        this.enemyGenerator = enemyGenerator;
    }
}
