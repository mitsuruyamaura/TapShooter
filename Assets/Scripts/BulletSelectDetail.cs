using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BulletSelectDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnBulletSelect = null;

    [SerializeField]
    private Image imgBulletBtn = null;

    public BulletDataSO.BulletData bulletData; // Debug�p�� public

    private PlayerController playerController;

    private BulletSelectManager bulletSelectManager;

    private float launchTime;  // ���˂ł��鎞��

    private float initialLaunchTime;   // ���˂ł��鎞�Ԃ̏����l

    private bool isLoading;    // ���U�����ǂ����Btrue �Ȃ瑕�U���Ŕ��˂ł���BlaunchTime ������

    public bool isDefaultBullet;  // �����o���b�g���ǂ����B���ˎ��ԂɊ֌W�Ȃ����˂ł���

    [SerializeField]
    private Image imgLaunchTimeGauge;  // ���˂ł��鎞�Ԃ̃Q�[�W

    [SerializeField]
    private Text txtOpenExpValue;

    private bool isCostPayment;        // ���݃R�X�g���x�����ĊJ���ς��ǂ����Btrue�Ȃ�R�X�g�x�����I��

    public bool isOpenAnimation;

    [SerializeField]
    private Image imgElementTypeBackground;

    /// <summary>
    /// �����ݒ�
    /// </summary>
    /// <param name="bulletData"></param>
    /// <param name="playerController"></param>
    /// <param name="bulletSelectManager"></param>
    public void SetUpBulletSelectDetail(BulletDataSO.BulletData bulletData, PlayerController playerController, BulletSelectManager bulletSelectManager) {
        this.bulletData = bulletData;
        this.playerController = playerController;
        this.bulletSelectManager = bulletSelectManager;

        // �摜�ύX
        imgBulletBtn.sprite = this.bulletData.btnSprite;

        // ���\�b�h�o�^
        btnBulletSelect.onClick.AddListener(OnClickBulletSelect);

        SwitchActivateBulletBtn(false);

        // ���ˎ��Ԑݒ�
        initialLaunchTime = this.bulletData.launchTime;
        this.launchTime = initialLaunchTime;

        // �Q�[�W���\���ɂ���
        imgLaunchTimeGauge.fillAmount = 0;

        // �J���\��EXP��\��
        txtOpenExpValue.text = this.bulletData.openExp.ToString();

        // �����o���b�g�m�F
        if (this.bulletData.openExp == 0) {
            // �����o���b�g�p�̐ݒ�
            isDefaultBullet = true;
            isLoading = true;
            txtOpenExpValue.gameObject.SetActive(false);

            ChangeColorToBulletBtn(new Color(0.65f, 0.65f, 0.65f));

            bulletSelectManager.ChangeDefenseBaseElementType(this.bulletData.elementType);
        }

        // �w�i�摜�̕ύX
        imgElementTypeBackground.sprite = DataBaseManager.instance.GetElementTypeSprite(this.bulletData.elementType);
    }

    /// <summary>
    /// �o���b�g�I��
    /// </summary>
    public void OnClickBulletSelect() {
        playerController.ChangeBullet(bulletData.bulletType);
        bulletSelectManager.ChangeLoadingBulletSettings(bulletData.bulletType);

        bulletSelectManager.ChangeDefenseBaseElementType(bulletData.elementType);

        // �d���h�~
        if (!isDefaultBullet && imgLaunchTimeGauge.fillAmount == 0) {
            // �Q�[�W���ő�l�ɂ���
            imgLaunchTimeGauge.fillAmount = 1.0f;

            // �R�X�g�x�����Ϗ�Ԃɂ��� = �J�������ɂ�EXP������Ȃ��Ă������Ȃ���ԂɂȂ�Ȃ��悤�ɂ���
            SetStateBulletCostPayment(true);

            Debug.Log(GetStateBulletCostPayment());

            // EXP���Z
            bulletSelectManager.UpdateTotalExp(-bulletData.openExp);

            // �R�X�gEXP��\��
            //txtOpenExpValue.enabled = false;     
            txtOpenExpValue.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ���U���̃o���b�g��؂�ւ�
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ChangeLoadingBullet(bool isSwitch) {
        isLoading = isSwitch;
    }

    /// <summary>
    /// �o���b�g���^�b�v�ۂ̍X�V
    /// </summary>
    public void SwitchActivateBulletBtn(bool isSwitch) {
        btnBulletSelect.interactable = isSwitch;
    }

    /// <summary>
    /// �{�^���̐F��ύX
    /// </summary>
    /// <param name="newColor"></param>
    public void ChangeColorToBulletBtn(Color newColor) {
        imgBulletBtn.color = newColor;
    }

    void Update() {

        // �����o���b�g�͔��˂ł��鎞�Ԑ����Ȃ�
        if (isDefaultBullet) {
            return;
        }

        // ���U���Ă���o���b�g�łȂ���Ή������Ȃ�
        if (!isLoading) {
            return;
        }

        // ���˂ł��鎞�Ԃ����炷
        launchTime -= Time.deltaTime;

        // �Q�[�W�����킹��
        imgLaunchTimeGauge.DOFillAmount(launchTime / initialLaunchTime, 0.25f);

        // ���˂ł��鎞�Ԃ��Ȃ��Ȃ�����
        if (launchTime <= 0) {
            launchTime = 0;

            InitialzeBulletState();
        }
    }

    /// <summary>
    /// �����o���b�g�ȊO�̃o���b�g��������Ԃɖ߂�
    /// </summary>
    private void InitialzeBulletState() {
        // ���U���~�߂�
        isLoading = false;

        // �����o���b�g�ɖ߂�
        bulletSelectManager.ActivateDefaultBullet();

        // ���ˎ��Ԃ̏�����
        launchTime = initialLaunchTime;

        // �J���ɕK�v��EXP��\��
        //txtOpenExpValue.enabled = true;
        txtOpenExpValue.gameObject.SetActive(true);

        // �R�X�g�������̏�Ԃɖ߂�
        SetStateBulletCostPayment(false);

        // 
        bulletSelectManager.JugdeOpenBullets();
    }

    /// <summary>
    /// �R�X�g�x������Ԃ̊m�F
    /// </summary>
    /// <returns></returns>
    public bool GetStateBulletCostPayment() {
        return isCostPayment;
    }

    /// <summary>
    /// �R�X�g�x������Ԃ̍X�V
    /// </summary>
    /// <param name="isSet"></param>
    public void SetStateBulletCostPayment(bool isSet) {
        isCostPayment = isSet;
    }

    /// <summary>
    /// �R�X�g���x�������ԂɂȂ����ۂ̃A�j�����o 
    /// </summary>
    public void OpenBulletAnimation(bool isSet) {
        isOpenAnimation = isSet;
        if (isOpenAnimation) {
            transform.DOShakeScale(0.25f, 1, 10, 45).SetEase(Ease.Linear);
        }
    }
}
