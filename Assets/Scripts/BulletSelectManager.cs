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
    /// バレット選択用ボタンの生成
    /// </summary>
    /// <param name="playerController"></param>
    /// <returns></returns>
    public IEnumerator GenerateBulletSelectDetail(PlayerController playerController) {

        List<BulletDataSO.BulletData> bulletDatas = new List<BulletDataSO.BulletData>();

        // 初期バレット生成用のデバッグスイッチがオンなら
        if (GameData.instance.isDebugDefaultBulletCreate) {
            bulletDatas = DataBaseManager.instance.bulletDataSO.bulletDataList.Where((x) => x.liberalType == BulletDataSO.LiberalType.Player).ToList();
        } else {
            // オフなら、バレット選択ポップアップで選択したバレットを利用する
            bulletDatas = GameData.instance.chooseBulletsList;
        }

        for (int i = 0; i < bulletDatas.Count; i++) {
            // バレットボタン生成
            BulletSelectDetail bulletSelectDetail = Instantiate(bulletSelectDetailPrefab, bulletTran, false);

            // バレットボタンの設定
            bulletSelectDetail.SetUpBulletSelectDetail(bulletDatas[i], playerController, this);

            // リストに追加
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
    /// EXPの値と表示更新
    /// </summary>
    public void UpdateTotalExp(int exp, ExpType expType) {

        // EXP の種類によりフロート表示
        if (expType == ExpType.BulletCost) {
            CreateFlotingMessageToBulletCost(exp, FloatingMessage.FloatingMessageType.BulletCost);
        } else {
            CreateFlotingMessageToBulletCost(exp, FloatingMessage.FloatingMessageType.GetExp);
        }

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
            //Debug.Log(bulletData.GetStateBulletCostPayment());

            if (gameManager.isGameUp) {
                bulletData.SwitchActivateBulletBtn(false);
                continue;
            }

            // コストを支払っているかどうか
            if (bulletData.GetStateBulletCostPayment()) {
                bulletData.SwitchActivateBulletBtn(true);
                continue;
            }

            if (bulletData.bulletData.openExp <= totalExp) {
                // 超えているものはタップできるようにする
                bulletData.SwitchActivateBulletBtn(true);

                if (!bulletData.isOpenAnimation) {
                    bulletData.OpenBulletAnimation(true);
                }
            } else {
                // 超えていないものはタップできないようにする
                bulletData.SwitchActivateBulletBtn(false);

                if (bulletData.isOpenAnimation) {
                    bulletData.OpenBulletAnimation(false);
                }
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
    private void CreateFlotingMessageToBulletCost(int exp, FloatingMessage.FloatingMessageType floatingMessageType) {
        FloatingMessage floatingMessage = Instantiate(floatingMessagePrefab, txtTotalExp.transform, false);
        floatingMessage.DisplayFloatingMessage(exp, floatingMessageType, false);
    }

    /// <summary>
    /// すべてのバレットボタンの操作を押せないように制御
    /// </summary>
    public void InactivateAllBulletBtns() {
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            bulletSelectDetailList[i].SwitchActivateBulletBtn(false);
        }
    }

    /// <summary>
    /// TotalExpを取得
    /// </summary>
    /// <returns></returns>
    public int GetTotalExp() {
        return totalExp;
    }

    public void AllSwitchActivateBulletBtn(bool isSwitch) {
        for (int i = 0; i < bulletSelectDetailList.Count; i++) {
            // 押せない状態にする
            bulletSelectDetailList[i].SwitchActivateBulletBtn(isSwitch);
        }
    }

    /// <summary>
    /// DefenceBase のElementType を変更
    /// </summary>
    public void ChangeDefenseBaseElementType(ElementType elementType) {
        defenseBase.ChangeElementType(elementType);
    }
}
