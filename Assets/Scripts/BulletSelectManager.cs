using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BulletSelectManager : MonoBehaviour
{
    [SerializeField]
    private BulletSelectDetail bulletSelectDetailPrefab;

    [SerializeField]
    private Transform bulletTran;

    public List<BulletSelectDetail> bulletSelectDetailList = new List<BulletSelectDetail>();

    private PlayerController playerController;

    public int totalExp;

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
    public void ChangeColorToBulletButton(BulletDataSO.BulletType bulletType) {
        Debug.Log(bulletType);
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            if (bulletSelectDetailList[i].bulletData.bulletType == bulletType) {

                // �I�𒆂͊D�F
                bulletSelectDetailList[i].imgBullet.color = new Color(0.65f, 0.65f, 0.65f);
            } else {
                // ���I��
                bulletSelectDetailList[i].imgBullet.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }

    /// <summary>
    /// EXP���Z
    /// </summary>
    public void UpdateTotalExp(int exp) {

        // EXP���Z
        totalExp += exp;

        // �g�p�\�o���b�g�̊m�F�ƍX�V
        JugdeOpenBullets();
    }

    /// <summary>
    /// �g�p�\�o���b�g�̊m�F�ƍX�V
    /// </summary>
    public void JugdeOpenBullets() {

        //bool isOpen = false;
        // �o���b�g���ƂɎg�p�\��EXP�𒴂��Ă��邩�m�F
        foreach (BulletSelectDetail bulletData in bulletSelectDetailList) {   // .Where((x) => x.bulletData.openExp <= totalExp ? false : true)
            if (bulletData.bulletData.openExp <= totalExp) {
                // �����Ă�����̂̓^�b�v�ł���悤�ɂ���
                bulletData.SwitchActivateBulletBtn(true);
            } else {
                bulletData.SwitchActivateBulletBtn(false);
            }

            //Debug.Log(isOpen);
        }
    }
}
