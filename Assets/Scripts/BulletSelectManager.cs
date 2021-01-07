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

    private PlayerController playerController;

    public int totalExp;

    [SerializeField]
    private Text txtTotalExp;

    [SerializeField]
    private FloatingMessage floatingDamagePrefab;

    //void Start()
    //{
    //    GenerateBulletSelectDetail();
    //}

    public IEnumerator GenerateBulletSelectDetail(PlayerController playerController) {
        BulletDataSO.BulletData[] bulletDatas = DataBaseManager.instance.bulletDataSO.bulletDataList.Where((x) => x.liberalType == BulletDataSO.LiberalType.Player).ToArray();

        for (int i = 0; i < bulletDatas.Length; i++) {
            BulletSelectDetail bulletSelectDetail = Instantiate(bulletSelectDetailPrefab, bulletTran, false);
            bulletSelectDetail.SetUpBulletSelectDetail(bulletDatas[i], playerController, this);
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
    /// EXP���Z
    /// </summary>
    public void UpdateTotalExp(int exp) {

        // �t���[�g�\��
        CreateFlotingExp(exp);

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
            if (bulletData.bulletData.openExp <= totalExp) {
                // �����Ă�����̂̓^�b�v�ł���悤�ɂ���
                bulletData.SwitchActivateBulletBtn(true);
            } else {
                // �����Ă��Ȃ����̂̓^�b�v�ł��Ȃ��悤�ɂ���
                bulletData.SwitchActivateBulletBtn(false);
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
    private void CreateFlotingExp(int exp) {
        FloatingMessage floatingDamage = Instantiate(floatingDamagePrefab, txtTotalExp.transform, false);
        floatingDamage.DisplayFloatingDamage(exp, FloatingMessage.FloatingMessageType.GetExp);
    }
}
