using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class BulletSelectManager : MonoBehaviour
{
    [SerializeField]
    private BulletSelectDetail bulletSelectDetailPrefab;

    [SerializeField]
    private Transform bulletTran;

    public List<BulletSelectDetail> bulletSelectDetailList = new List<BulletSelectDetail>();

    public int totalExp;

    [SerializeField]
    private Text txtTotalExp;

    [SerializeField]
    private FloatingMessage floatingMessagePrefab;

    [SerializeField]
    private DefenseBase defenseBase;

    [SerializeField]
    private GameManager gameManager;

    public enum ExpType {
        BulletCost,
        EnemyExp
    }

    //void Start()
    //{
    //    GenerateBulletSelectDetail();
    //}

    /// <summary>
    /// �o���b�g�I��p�{�^���̐���
    /// </summary>
    /// <param name="playerController"></param>
    /// <returns></returns>
    public IEnumerator GenerateBulletSelectDetail(PlayerController playerController) {

        List<BulletDataSO.BulletData> bulletDatas = new List<BulletDataSO.BulletData>();

        // �����o���b�g�����p�̃f�o�b�O�X�C�b�`���I���Ȃ�
        if (GameData.instance.isDebugDefaultBulletCreate) {
            bulletDatas = DataBaseManager.instance.bulletDataSO.bulletDataList.Where((x) => x.liberalType == BulletDataSO.LiberalType.Player).ToList();
        } else {
            // �I�t�Ȃ�A�o���b�g�I���|�b�v�A�b�v�őI�������o���b�g�𗘗p����
            bulletDatas = GameData.instance.chooseBulletsList;
        }

        for (int i = 0; i < bulletDatas.Count; i++) {
            // �o���b�g�{�^������
            BulletSelectDetail bulletSelectDetail = Instantiate(bulletSelectDetailPrefab, bulletTran, false);

            // �o���b�g�{�^���̐ݒ�
            bulletSelectDetail.SetUpBulletSelectDetail(bulletDatas[i], playerController, this);

            // ���X�g�ɒǉ�
            bulletSelectDetailList.Add(bulletSelectDetail);
            yield return new WaitForSeconds(0.25f);
        }
    } 

    /// <summary>
    /// �I�����Ă���o���b�g�̐F��ύX
    /// </summary>
    /// <param name="bulletType"></param>
    public void ChangeLoadingBulletSettings(BulletDataSO.BulletType bulletType) {
        Debug.Log(bulletType);
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            if (bulletSelectDetailList[i].bulletData.bulletType == bulletType) {

                // �I�𒆂͊D�F
                bulletSelectDetailList[i].ChangeColorToBulletBtn(new Color(0.65f, 0.65f, 0.65f));

                bulletSelectDetailList[i].ChangeLoadingBullet(true);
            } else {
                // ���I��
                bulletSelectDetailList[i].ChangeColorToBulletBtn(new Color(1.0f, 1.0f, 1.0f));
                bulletSelectDetailList[i].ChangeLoadingBullet(false);
            }
        }
    }

    /// <summary>
    /// EXP�̒l�ƕ\���X�V
    /// </summary>
    public void UpdateTotalExp(int exp, ExpType expType) {

        // EXP �̎�ނɂ��t���[�g�\��
        if (expType == ExpType.BulletCost) {
            CreateFlotingMessageToBulletCost(exp, FloatingMessage.FloatingMessageType.BulletCost);
        } else {
            CreateFlotingMessageToBulletCost(exp, FloatingMessage.FloatingMessageType.GetExp);
        }

        // EXP���Z
        //totalExp += exp;

        int currentAp = totalExp;
        int updateAp = currentAp + exp;

        // �\���X�V     
        DOTween.To(
            () => currentAp,
            (x) => {
                currentAp = x;
                txtTotalExp.text = x.ToString();
            },
            updateAp,
            1.0f);
        totalExp = updateAp;

        // �g�p�\�o���b�g�̊m�F�ƍX�V
        JugdeOpenBullets();
    }

    /// <summary>
    /// �g�p�\�o���b�g�̊m�F�ƍX�V
    /// </summary>
    public void JugdeOpenBullets() {

        // �o���b�g���ƂɎg�p�\��EXP�𒴂��Ă��邩�m�F
        foreach (BulletSelectDetail bulletData in bulletSelectDetailList) {
            //Debug.Log(bulletData.GetStateBulletCostPayment());

            if (gameManager.isGameUp) {
                bulletData.SwitchActivateBulletBtn(false);
                continue;
            }

            // �R�X�g���x�����Ă��邩�ǂ���
            if (bulletData.GetStateBulletCostPayment()) {
                bulletData.SwitchActivateBulletBtn(true);
                continue;
            }

            if (bulletData.bulletData.openExp <= totalExp) {
                // �����Ă�����̂̓^�b�v�ł���悤�ɂ���
                bulletData.SwitchActivateBulletBtn(true);

                if (!bulletData.isOpenAnimation) {
                    bulletData.OpenBulletAnimation(true);
                }
            } else {
                // �����Ă��Ȃ����̂̓^�b�v�ł��Ȃ��悤�ɂ���
                bulletData.SwitchActivateBulletBtn(false);

                if (bulletData.isOpenAnimation) {
                    bulletData.OpenBulletAnimation(false);
                }
            }
        }
    }

    /// <summary>
    /// �����o���b�g�ݒ�
    /// </summary>
    public void ActivateDefaultBullet() {
        foreach (BulletSelectDetail bulletSelectDetail in bulletSelectDetailList) {
            if (bulletSelectDetail.isDefaultBullet) {
                bulletSelectDetail.OnClickBulletSelect();
                Debug.Log("�����o���b�g�ɐݒ�");
                return;
            }
        }
    }

    /// <summary>
    /// �l������EXP���t���[�g�\��
    /// </summary>
    /// <param name="exp"></param>
    private void CreateFlotingMessageToBulletCost(int exp, FloatingMessage.FloatingMessageType floatingMessageType) {
        FloatingMessage floatingMessage = Instantiate(floatingMessagePrefab, txtTotalExp.transform, false);
        floatingMessage.DisplayFloatingMessage(exp, floatingMessageType, false);
    }

    /// <summary>
    /// ���ׂẴo���b�g�{�^���̑���������Ȃ��悤�ɐ���
    /// </summary>
    public void InactivateAllBulletBtns() {
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            bulletSelectDetailList[i].SwitchActivateBulletBtn(false);
        }
    }

    /// <summary>
    /// TotalExp���擾
    /// </summary>
    /// <returns></returns>
    public int GetTotalExp() {
        return totalExp;
    }

    public void AllSwitchActivateBulletBtn(bool isSwitch) {
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            // �����Ȃ���Ԃɂ���
            bulletSelectDetailList[i].SwitchActivateBulletBtn(isSwitch);
        }
    }

    /// <summary>
    /// DefenceBase ��ElementType ��ύX
    /// </summary>
    public void ChangeDefenseBaseElementType(ElementType elementType) {
        defenseBase.ChangeElementType(elementType);
    }
}
