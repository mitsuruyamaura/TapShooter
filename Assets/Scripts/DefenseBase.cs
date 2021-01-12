using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DefenseBase : MonoBehaviour {
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text txtDurability;

    public int durability;

    private int maxDurability;

    [SerializeField]
    private CanvasGroup canvasGroupGameOver;

    [SerializeField]
    private Text txtGameOver;

    private GameManager gameManager;

    [SerializeField]
    private FloatingMessage floatingMessagePrefab;

    public ElementType currentElementType;            // Debug終わったら privateにする

    [SerializeField]
    private Transform floatingMessageTran;

    [SerializeField]
    private GameObject bulletHitEffectPrefab;

    [SerializeField]
    private GameObject enemyAttackEffectPrefab;

    [SerializeField]
    private Button btnRestartFilter;


    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        canvasGroupGameOver.DOFade(0, 0.1f);
        maxDurability = durability;

        UpdateDisplayDurability();

        btnRestartFilter.onClick.AddListener(() => gameManager.OnClickRestart(canvasGroupGameOver));
    }

    /// <summary>
    /// 耐久度の表示更新
    /// </summary>
    private void UpdateDisplayDurability() {
        txtDurability.text = durability + "  / " + maxDurability;

        slider.DOValue((float)durability / maxDurability, 0.25f);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Enemy") {

            col.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            GameObject effectPrefab = null;
            if (col.gameObject.TryGetComponent(out Bullet bullet)) {
                // 相性確認してダメージ処理
                Damage(bullet.bulletData.bulletPower, bullet.bulletData.elementType);

                Debug.Log("bullet");

                effectPrefab = bulletHitEffectPrefab;

            } else if (col.gameObject.TryGetComponent(out EnemyController enemyController)) {
                Damage(enemyController.enemyData.power, enemyController.enemyData.elementType);

                Debug.Log("Enemy");

                effectPrefab = enemyAttackEffectPrefab;
            }

            // 種類に合わせたエフェクトを生成
            GameObject effect = Instantiate(effectPrefab, col.gameObject.transform, false);
            //effect.transform.SetParent(GameObject.FindGameObjectWithTag("BulletPool").transform);
            effect.transform.SetParent(DataBaseManager.instance.GetTemporaryObjectContainerTransform());
            Destroy(effect, 3.0f);

            // TODO SE

            durability = Mathf.Clamp(durability, 0, maxDurability);
            UpdateDisplayDurability();

            if (durability <= 0 && gameManager.isGameUp == false) {
                Debug.Log("Game Over");
                gameManager.SwitchGameUp(true);

                DisplayGameOver();
            }
            Destroy(col.gameObject);
        }
    }

    /// <summary>
    /// ゲームオーバー表示
    /// </summary>
    private void DisplayGameOver() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupGameOver.DOFade(1.0f, 1.0f));
        string txt = "Game Over";
        sequence.Append(txtGameOver.DOText(txt, 1.5f))
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                // 画面のタップを許可
                canvasGroupGameOver.blocksRaycasts = true;
            });
    }

    /// <summary>
    /// ElementType変更
    /// </summary>
    /// <param name="newElementType"></param>
    public void ChangeElementType(ElementType newElementType) {
        currentElementType = newElementType;

        Debug.Log("現在のElementType : " + currentElementType);
    }

    /// <summary>
    /// ElementTypeの相性判定を行ってダメージ処理
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackElementType"></param>
    private void Damage(int damage, ElementType attackElementType) {
        // 相性確認
        if (ElementCompatibilityHelper.GetElementCompatibility(attackElementType, currentElementType)) {
            durability -= damage * 2;
            CreateFloatingDamage(damage * 2, true);

            Debug.Log("エネミーと DefenseBase の Element 相性　悪い");
        } else {
            durability -= damage;
            CreateFloatingDamage(damage, false);
            Debug.Log("エネミーと DefenseBase の Element 相性　普通");
        }
    }

    /// <summary>
    /// ダメージ表示の生成
    /// </summary>
    /// <param name="bulletPower"></param>
    /// <param name="isElementCompatibility"></param>
    private void CreateFloatingDamage(int bulletPower, bool isElementCompatibility) {
        FloatingMessage floatingMessage = Instantiate(floatingMessagePrefab, floatingMessageTran);
        floatingMessage.DisplayFloatingMessage(bulletPower, FloatingMessage.FloatingMessageType.PlayerDamage, isElementCompatibility);
    }
}

