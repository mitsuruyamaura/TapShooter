using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class ChooseBulletPopUp : MonoBehaviour
{
    [SerializeField]
    private Text txtBulletDescription;

    // 未選択のバレットのリスト
    public List<ChooseBulletDetail> unselectedBulletDetailsList = new List<ChooseBulletDetail>();

    // 登録中のバレットのリスト
    public List<ChooseBulletDetail> choosingBulletDetailsList = new List<ChooseBulletDetail>();

    [SerializeField]
    private ChooseBulletDetail chooseBulletDetailPrefab;

    [SerializeField]
    private Transform bulletDetailTran;

    [SerializeField]
    private Transform selectBulletTran;

    void Start()
    {
        CreateChooseBulletDetails();
    }

    /// <summary>
    /// バレットのリスト作成
    /// </summary>
    public void CreateChooseBulletDetails() {
        // プレイヤーのバレットのみ抽出
        BulletDataSO.BulletData[] bulletDatas = DataBaseManager.instance.bulletDataSO.bulletDataList.Where((x) => x.liberalType == BulletDataSO.LiberalType.Player).ToArray();

        Debug.Log(bulletDatas.Length);

        // バレット生成
        for (int i = 0; 0 < bulletDatas.Length; i++) {
            if (i == 4) {
                return;
            }
            ChooseBulletDetail chooseBulletDetail = Instantiate(chooseBulletDetailPrefab, selectBulletTran, false);
            chooseBulletDetail.SetUpChooseBullet(this, ChooseBulletDetail.DetailStateType.Unselected, bulletDatas[i]);
            unselectedBulletDetailsList.Add(chooseBulletDetail);
        }
    }

    /// <summary>
    /// バレットの性能の表示更新
    /// </summary>
    /// <param name="bulletData"></param>
    public void UpdateDisplayBulletPerformance(BulletDataSO.BulletData bulletData) {
        txtBulletDescription.text = bulletData.discription;

    }

    /// <summary>
    /// 利用可能なバレットのリストに追加
    /// </summary>
    public void AddChooseBulletDetail(ChooseBulletDetail chooseBulletDetail) {
        // 生成
        ChooseBulletDetail chooseBulletDetailCopy = Instantiate(chooseBulletDetail, bulletDetailTran, false);

        chooseBulletDetailCopy.SetUpChooseBullet(this, ChooseBulletDetail.DetailStateType.Choosing, chooseBulletDetailCopy.bulletData);
        choosingBulletDetailsList.Add(chooseBulletDetailCopy);
    }

    /// <summary>
    /// 選択したバレットをリストから削除
    /// </summary>
    /// <param name="chooseBulletDetail"></param>
    public void DeleteChoosingBulletList(ChooseBulletDetail chooseBulletDetail) {
        // 登録リストから、選択したバレットの情報を検索する
        foreach (ChooseBulletDetail chooseBullet in unselectedBulletDetailsList) {
            if (chooseBullet.bulletData.bulletType == chooseBulletDetail.bulletData.bulletType) {

                // タップできる状態に戻す
                chooseBullet.ActivateBtn(true);

                // ステートを初期化
                chooseBullet.SetState(ChooseBulletDetail.DetailStateType.Unselected);
            }
        }

        // 選択リストから選択したバレットを削除
        Destroy(chooseBulletDetail.gameObject);

        // 選択したバレットをリストから削除
        choosingBulletDetailsList.Remove(chooseBulletDetail);
    }

    /// <summary>
    /// 選択しているバレットリストをすべて削除
    /// </summary>
    public void ResetChoosingBulletList() {
        if (choosingBulletDetailsList.Count > 0) {
            return;
        }

        for (int i = 0; 0 < choosingBulletDetailsList.Count; i++) {
            Destroy(choosingBulletDetailsList[i]);
        }

        choosingBulletDetailsList.Clear();
    }

    /// <summary>
    /// 選択中の StateType を Unselected に戻す
    /// </summary>
    public void ResetStateTypeAllBtn(ChooseBulletDetail.DetailStateType newStateType) {
        for (int i = 0; i < unselectedBulletDetailsList.Count; i++) {
            if (unselectedBulletDetailsList[i].stateType == ChooseBulletDetail.DetailStateType.Selected) {
                unselectedBulletDetailsList[i].SetState(newStateType);

                unselectedBulletDetailsList[i].SwitchFrame(false);
            }
        }
    }
}
