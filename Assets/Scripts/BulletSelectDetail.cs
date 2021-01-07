using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BulletSelectDetail : MonoBehaviour
{
    public Button btnBulletSelect;

    public Image imgBullet;

    public BulletDataSO.BulletData bulletData; // Debug�p�� public

    private PlayerController playerController;

    private BulletSelectManager bulletSelectManager;

    private float launchTime;  // ���˂ł��鎞��

    private float initialLaunchTime;   // ���˂ł��鎞�Ԃ̏����l

    private bool isLoading;    // ���U�����ǂ����Btrue �Ȃ瑕�U���Ŕ��˂ł���BlaunchTime ������

    public bool isDefaultBullet;  // �����o���b�g���ǂ����B���ˎ��ԂɊ֌W�Ȃ����˂ł���

    [SerializeField]
    private Image imgLaunchTimeGauge;  // ���˂ł��鎞�Ԃ̃Q�[�W

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
        imgBullet.sprite = this.bulletData.sprite;

        // ���\�b�h�o�^
        btnBulletSelect.onClick.AddListener(OnClickBulletSelect);

        // ���ˎ��Ԑݒ�
        initialLaunchTime = this.bulletData.launchTime;
        this.launchTime = initialLaunchTime;

        // �Q�[�W���\���ɂ���
        imgLaunchTimeGauge.fillAmount = 0;

        // �����o���b�g�m�F
        if (this.bulletData.openExp == 0) {
            // �����o���b�g�p�̐ݒ�
            isDefaultBullet = true;
            isLoading = true;
            ChangeColorToBulletBtn(new Color(0.65f, 0.65f, 0.65f));
        }
    }

    /// <summary>
    /// �o���b�g�I��
    /// </summary>
    public void OnClickBulletSelect() {
        playerController.ChangeBullet(bulletData.bulletType);
        bulletSelectManager.ChangeLoadingBulletSettings(bulletData.bulletType);

        // �d���h�~
        if (!isDefaultBullet && imgLaunchTimeGauge.fillAmount == 0) {
            // �Q�[�W���ő�l�ɂ���
            imgLaunchTimeGauge.fillAmount = 1.0f;
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
        imgBullet.color = newColor;
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

            // ���U���~�߂�
            isLoading = false;

            // �����o���b�g�ɖ߂�
            bulletSelectManager.ActivateDefaultBullet();

            launchTime = initialLaunchTime;
        }
    }
}
