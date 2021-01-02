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
    private Slider slider;

    [SerializeField]
    private CanvasGroup canvasGroupSlider;

    [SerializeField]
    private Transform floatingDamageTran;

    [SerializeField]
    private FloatingDamage floatingDamagePrefab;

    private int maxHp;

    void Start() {
        
        maxHp = enemyData.hp;
        UpDateDisplayHpGauge();
    }

    void Update() {
        transform.Translate(0, -0.01f, 0);
        if (transform.localPosition.y < -1500) {
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
        enemyData.hp -= bullet.bulletData.bulletPower;
        Debug.Log("Hp : " + enemyData.hp);

        CreateFloatingDamage(bullet.bulletData.bulletPower);
        UpDateDisplayHpGauge();

        if (enemyData.hp <= 0) {
            enemyData.hp = 0;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// HPゲージの表示更新
    /// </summary>
    private void UpDateDisplayHpGauge() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupSlider.DOFade(1.0f, 0.15f));
        sequence.Join(slider.DOValue((float)enemyData.hp / maxHp, 0.25f));
        sequence.AppendInterval(0.25f);
        sequence.Append(canvasGroupSlider.DOFade(0f, 0.15f));
    }

    /// <summary>
    /// ダメージ表示の生成
    /// </summary>
    /// <param name="bulletPower"></param>
    private void CreateFloatingDamage(int bulletPower) {
        FloatingDamage floatingDamage = Instantiate(floatingDamagePrefab, floatingDamageTran);
        floatingDamage.DisplayFloatingDamage(bulletPower);
    }


    public void Inisialize(PlayerController playerController, BulletDataSO.BulletData bulletData, EnemyDataSO.EnemyData enemyData, Bullet bulletPrefab = null)
    {
        transform.localPosition = new Vector3(transform.localPosition.x + Random.Range(-650, 650), transform.localPosition.y, 0);

        this.playerController = playerController;
        this.enemyData = enemyData;
        this.bulletPrefab = bulletPrefab;

        if (bulletPrefab != null) {

            //
            StartCoroutine(EnemyShot(bulletData));
        }

        moveEvent = DataBaseManager.instance.moveDataSO.GetMoveEvent(enemyData.enemyType);

        moveEvent.Invoke(transform);
    }

    private IEnumerator EnemyShot(BulletDataSO.BulletData bulletData) {

        while (true) {
            Instantiate(bulletPrefab, transform).Shot(bulletData, GetPlayerDirection());

            yield return new WaitForSeconds(bulletData.loadingTime);
        }
    }

    private Vector3 GetPlayerDirection() {
        return (playerController.transform.position - transform.position).normalized;
    }
}
