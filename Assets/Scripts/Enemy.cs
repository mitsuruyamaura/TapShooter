using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Enemy : MonoBehaviour
{
    // �e�̃N���X
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

    [Header("�󔠂̏o���m��"), Range(0, 100)]
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
    /// HP�X�V
    /// </summary>
    /// <param name="bullet"></param>
    private void UpdateHp(Bullet bullet) {
        if(ElementCompatibilityHelper.GetElementCompatibility(bullet.bulletData.elementType, enemyData.elementType) == true) {
            hp -= bullet.bulletData.bulletPower * 2;
            Debug.Log("Element �����@��");

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

            // EXP���Z
            enemyGenerator.UpdateExp(enemyData.exp);

            // �󔠐�������
            if (JudgeGenerateTrasureBox()) {
                
                // �󔠐���
                GenerateTreasureBox();
            }

            Destroy(gameObject);
        }

        // �m�[�}���e�̏ꍇ
        if (bullet.bulletData.bulletType == BulletDataSO.BulletType.Player_Normal || bullet.bulletData.bulletType == BulletDataSO.BulletType.Player_5ways_Normal) {
            // �j��
            Destroy(bullet.gameObject);
        }
    }

    /// <summary>
    /// HP�Q�[�W�̕\���X�V
    /// </summary>
    private void UpDateDisplayHpGauge() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupSlider.DOFade(1.0f, 0.15f));
        sequence.Join(slider.DOValue((float)hp / maxHp, 0.25f));
        sequence.AppendInterval(0.25f);
        sequence.Append(canvasGroupSlider.DOFade(0f, 0.15f));
    }

    /// <summary>
    /// �_���[�W�\���̐���
    /// </summary>
    /// <param name="bulletPower"></param>
    private void CreateFloatingMessageToDamage(int bulletPower, bool isElementCompatibility) {
        FloatingMessage floatingMessage = Instantiate(floatingMessagePrefab, floatingDamageTran);
        floatingMessage.DisplayFloatingMessage(bulletPower, FloatingMessage.FloatingMessageType.EnemyDamage , isElementCompatibility);
    }

    /// <summary>
    /// ������
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

            // �e�̎�������          
            StartCoroutine(EnemyShot(bulletData));
        }

        moveEvent = DataBaseManager.instance.moveDataSO.GetMoveEvent(enemyData.enemyType);

        moveEvent.Invoke(transform);
    }

    /// <summary>
    /// �e�̎�������
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
    /// �v���C���[�ƃG�l�~�[�Ƃ̈ʒu��������𔻒�
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerDirection() {
        return (playerController.transform.position - transform.position).normalized;
    }

    /// <summary>
    /// �ǉ��̏����ݒ�
    /// </summary>
    /// <param name="enemyGenerator"></param>
    public void AdditionalInitialize(EnemyGenerator enemyGenerator) {
        this.enemyGenerator = enemyGenerator;
    }

    /// <summary>
    /// ��o���b�g���̃q�b�g���o�p�̃G�t�F�N�g����
    /// </summary>
    /// <param name="tran"></param>
    private void GenerateBulletEffect(Transform tran) {
        GameObject effect = Instantiate(bulletEffectPrefab, tran, false);
        //effect.transform.SetParent(GameObject.FindGameObjectWithTag("BulletPool").transform);
        effect.transform.SetParent(DataBaseManager.instance.GetTemporaryObjectContainerTransform());
        Destroy(effect, 3.0f);
    }

    /// <summary>
    /// �󔠂��o�����邩����
    /// </summary>
    /// <returns></returns>
    private bool JudgeGenerateTrasureBox() {
        return Random.Range(0, 100) < appearTreasureRate ? true : false;
    }

    /// <summary>
    /// �󔠐���
    /// </summary>
    private void GenerateTreasureBox() {
        TreasureBox treasureBox = Instantiate(treasureBoxPrefab, transform, false);
        treasureBox.transform.SetParent(DataBaseManager.instance.GetTemporaryObjectContainerTransform());
        treasureBox.SetUpTreasureBox(enemyGenerator);
    }
}
