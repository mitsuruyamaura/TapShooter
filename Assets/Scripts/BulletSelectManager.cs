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
    /// 選択しているバレットの色を変更
    /// </summary>
    /// <param name="bulletType"></param>
    public void ChangeLoadingBulletSettings(BulletDataSO.BulletType bulletType) {
        Debug.Log(bulletType);
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            if (bulletSelectDetailList[i].bulletData.bulletType == bulletType) {

                // 選択中は灰色
                bulletSelectDetailList[i].ChangeColorToBulletBtn(new Color(0.65f, 0.65f, 0.65f));

                bulletSelectDetailList[i].ChangeLoadingBullet(true);
            } else {
                // 未選択
                bulletSelectDetailList[i].ChangeColorToBulletBtn(new Color(1.0f, 1.0f, 1.0f));
                bulletSelectDetailList[i].ChangeLoadingBullet(false);
            }
        }
    }

    /// <summary>
    /// EXP加算
    /// </summary>
    public void UpdateTotalExp(int exp) {

        // フロート表示
        CreateFlotingExp(exp);

        // EXP加算
        //totalExp += exp;

        int currentAp = totalExp;
        int updateAp = currentAp + exp;

        // 表示更新     
        DOTween.To(
            () => currentAp,
            (x) => {
                currentAp = x;
                txtTotalExp.text = x.ToString();
            },
            updateAp,
            1.0f);
        totalExp = updateAp;

        // 使用可能バレットの確認と更新
        JugdeOpenBullets();
    }

    /// <summary>
    /// 使用可能バレットの確認と更新
    /// </summary>
    public void JugdeOpenBullets() {

        // バレットごとに使用可能なEXPを超えているか確認
        foreach (BulletSelectDetail bulletData in bulletSelectDetailList) {
            if (bulletData.bulletData.openExp <= totalExp) {
                // 超えているものはタップできるようにする
                bulletData.SwitchActivateBulletBtn(true);
            } else {
                // 超えていないものはタップできないようにする
                bulletData.SwitchActivateBulletBtn(false);
            }
        }
    }

    /// <summary>
    /// 初期バレット設定
    /// </summary>
    public void ActivateDefaultBullet() {
        foreach (BulletSelectDetail bulletSelectDetail in bulletSelectDetailList) {
            if (bulletSelectDetail.isDefaultBullet) {
                bulletSelectDetail.OnClickBulletSelect();
                Debug.Log("初期バレットに設定");
                return;
            }
        }
    }

    /// <summary>
    /// 獲得したEXPをフロート表示
    /// </summary>
    /// <param name="exp"></param>
    private void CreateFlotingExp(int exp) {
        FloatingMessage floatingDamage = Instantiate(floatingDamagePrefab, txtTotalExp.transform, false);
        floatingDamage.DisplayFloatingDamage(exp, FloatingMessage.FloatingMessageType.GetExp);
    }
}
