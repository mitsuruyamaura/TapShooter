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

    public ElementType currentElementType;            // Debug�I������� private�ɂ���

    [SerializeField]
    private Transform floatingMessageTran;

    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        canvasGroupGameOver.DOFade(0, 0.1f);
        maxDurability = durability;

        UpdateDisplayDurability();
    }

    /// <summary>
    /// �ϋv�x�̕\���X�V
    /// </summary>
    private void UpdateDisplayDurability() {
        txtDurability.text = durability + "  / " + maxDurability;

        slider.DOValue((float)durability / maxDurability, 0.25f);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Enemy") {

            col.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

            if (col.gameObject.TryGetComponent(out Bullet bullet)) {
                // �����m�F���ă_���[�W����
                Damage(bullet.bulletData.bulletPower, bullet.bulletData.elementType);

                Debug.Log("bullet");

            } else if (col.gameObject.TryGetComponent(out EnemyController enemyController)) {
                Damage(enemyController.enemyData.power, enemyController.enemyData.elementType);

                Debug.Log("Enemy");
            }

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
    /// �Q�[���I�[�o�[�\��
    /// </summary>
    private void DisplayGameOver() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupGameOver.DOFade(1.0f, 1.0f));
        string txt = "Game Over";
        sequence.Append(txtGameOver.DOText(txt, 1.5f)).SetEase(Ease.Linear);
    }

    /// <summary>
    /// ElementType�ύX
    /// </summary>
    /// <param name="newElementType"></param>
    public void ChangeElementType(ElementType newElementType) {
        currentElementType = newElementType;

        Debug.Log("���݂�ElementType : " + currentElementType);
    }

    /// <summary>
    /// ElementType�̑���������s���ă_���[�W����
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackElementType"></param>
    private void Damage(int damage, ElementType attackElementType) {
        // �����m�F
        if (ElementCompatibilityChecker.GetElementCompatibility(attackElementType, currentElementType)) {
            durability -= damage * 2;
            CreateFloatingDamage(damage * 2, true);

            Debug.Log("�G�l�~�[�� DefenseBase �� Element �����@����");
        } else {
            durability -= damage;
            CreateFloatingDamage(damage, false);
            Debug.Log("�G�l�~�[�� DefenseBase �� Element �����@����");
        }
    }

    /// <summary>
    /// �_���[�W�\���̐���
    /// </summary>
    /// <param name="bulletPower"></param>
    /// <param name="isElementCompatibility"></param>
    private void CreateFloatingDamage(int bulletPower, bool isElementCompatibility) {
        FloatingMessage floatingMessage = Instantiate(floatingMessagePrefab, floatingMessageTran);
        floatingMessage.DisplayFloatingDamage(bulletPower, FloatingMessage.FloatingMessageType.PlayerDamage, isElementCompatibility);
    }
}

