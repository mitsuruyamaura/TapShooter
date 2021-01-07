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
    /// 選択しているバレットの色を変更
    /// </summary>
    /// <param name="bulletType"></param>
    public void ChangeColorToBulletButton(BulletDataSO.BulletType bulletType) {
        Debug.Log(bulletType);
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            if (bulletSelectDetailList[i].bulletData.bulletType == bulletType) {

                // 選択中は灰色
                bulletSelectDetailList[i].imgBullet.color = new Color(0.65f, 0.65f, 0.65f);
            } else {
                // 未選択
                bulletSelectDetailList[i].imgBullet.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }

    /// <summary>
    /// EXP加算
    /// </summary>
    public void UpdateTotalExp(int exp) {

        // EXP加算
        totalExp += exp;

        // 使用可能バレットの確認と更新
        JugdeOpenBullets();
    }

    /// <summary>
    /// 使用可能バレットの確認と更新
    /// </summary>
    public void JugdeOpenBullets() {

        //bool isOpen = false;
        // バレットごとに使用可能なEXPを超えているか確認
        foreach (BulletSelectDetail bulletData in bulletSelectDetailList) {   // .Where((x) => x.bulletData.openExp <= totalExp ? false : true)
            if (bulletData.bulletData.openExp <= totalExp) {
                // 超えているものはタップできるようにする
                bulletData.SwitchActivateBulletBtn(true);
            } else {
                bulletData.SwitchActivateBulletBtn(false);
            }

            //Debug.Log(isOpen);
        }
    }
}
