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
    /// �ϋv�x�̕\���X�V
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
                // �����m�F���ă_���[�W����
                Damage(bullet.bulletData.bulletPower, bullet.bulletData.elementType);

                Debug.Log("bullet");

                effectPrefab = bulletHitEffectPrefab;

            } else if (col.gameObject.TryGetComponent(out EnemyController enemyController)) {
                Damage(enemyController.enemyData.power, enemyController.enemyData.elementType);

                Debug.Log("Enemy");

                effectPrefab = enemyAttackEffectPrefab;
            }

            // ��ނɍ��킹���G�t�F�N�g�𐶐�
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
    /// �Q�[���I�[�o�[�\��
    /// </summary>
    private void DisplayGameOver() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupGameOver.DOFade(1.0f, 1.0f));
        string txt = "Game Over";
        sequence.Append(txtGameOver.DOText(txt, 1.5f))
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                // ��ʂ̃^�b�v������
                canvasGroupGameOver.blocksRaycasts = true;
            });
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
        if (ElementCompatibilityHelper.GetElementCompatibility(attackElementType, currentElementType)) {
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
        floatingMessage.DisplayFloatingMessage(bulletPower, FloatingMessage.FloatingMessageType.PlayerDamage, isElementCompatibility);
    }
}

