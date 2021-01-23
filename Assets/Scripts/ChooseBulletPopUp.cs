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

    // 選択可能なアイテムバレットのリスト
    public List<ItemBulletButton> itemBulletsList = new List<ItemBulletButton>();

    // 登録中のアイテムバレットのリスト。これがゲームに反映される
    public List<ItemBulletButton> choosingItemBulletsList = new List<ItemBulletButton>();

    [SerializeField]
    private ItemBulletButton itemBulletButtonPrefab;

    [SerializeField, Header("選択可能なアイテムバレットの生成位置の情報")]
    private Transform itemBulletButtonTran;

    [SerializeField, Header("登録中のアイテムバレットの生成位置の情報")]
    private Transform choosingItemBulletTran;

    [SerializeField, Header("アイテムバレット確定ボタン")]
    private Button btnConfirmItemBullet;

    [SerializeField]
    private CanvasGroup canvasGroupPopUp;

    private GameManager gameManager;

    private int choosingItemBulletCount = 4;     // 登録可能なアイテムバレットの数


    void Start()
    {
        //CreateChooseBulletDetails();
    }

    /// <summary>
    /// アイテムバレットの生成
    /// </summary>
    public void CreateChooseBulletDetails(GameManager gameManager) {
        this.gameManager = gameManager;

        // アイテムバレット確定ボタンを非活性化
        btnConfirmItemBullet.interactable = false;

        // 徐々にポップアップを表示
        canvasGroupPopUp.DOFade(1.0f, 1.0f);

        // バレットデータから、プレイヤーのバレットデータのみ抽出して配列にする
        BulletDataSO.BulletData[] bulletDatas = DataBaseManager.instance.bulletDataSO.bulletDataList.Where((x) => x.liberalType == BulletDataSO.LiberalType.Player).ToArray();

        for (int i = 0; i < bulletDatas.Length; i++) {
            // アイテムバレット生成
            ItemBulletButton itemBulletButton = Instantiate(itemBulletButtonPrefab, itemBulletButtonTran, false);

            // アイテムバレットの設定
            itemBulletButton.SetUpChooseBullet(this, ItemBulletButton.ItemBulletStateType.Unselected, bulletDatas[i]);

            // リストに追加
            itemBulletsList.Add(itemBulletButton);
        }

        // アイテムバレット確定ボタンにメソッドを登録
        btnConfirmItemBullet.onClick.AddListener(OnClickConfirmItemBullets);
        btnConfirmItemBullet.onClick.AddListener(CloseChooseBulletPopUp);
    }

    /// <summary>
    /// バレットの性能の表示更新
    /// </summary>
    /// <param name="bulletData"></param>
    public void UpdateDisplayBulletPerformance(BulletDataSO.BulletData bulletData) {
        // バレットの性能の表示更新
        txtBulletDescription.text = bulletData.discription;
    }

    /// <summary>
    /// 選択したアイテムバレットを登録中のアイテムバレットのリストに追加
    /// </summary>
    public void AddChooseBulletDetail(ItemBulletButton chooseBulletDetail) {
        // アイテムバレットのコピー生成
        ItemBulletButton chooseBulletDetailCopy = Instantiate(chooseBulletDetail, choosingItemBulletTran, false);

        // アイテムバレットの設定
        chooseBulletDetailCopy.SetUpChooseBullet(this, ItemBulletButton.ItemBulletStateType.Choosing, chooseBulletDetailCopy.bulletData);
        choosingItemBulletsList.Add(chooseBulletDetailCopy);

        // TODO デフォルトのバレットが１つ選択されているか？ => メッセージ出す


        // アイテムバレットが登録リストに指定数登録されているか
        if (choosingItemBulletsList.Count == choosingItemBulletCount) {

            // アイテムバレット確定ボタンを活性化
            btnConfirmItemBullet.interactable = true;

            // アイテムバレット確定ボタンのアニメ演出(押せる状態になったことを知らせる)
            btnConfirmItemBullet.transform.DOShakeScale(0.5f);
        } 
    }

    /// <summary>
    /// 登録リストから選択したアイテムバレットを削除
    /// </summary>
    /// <param name="chooseBulletDetail"></param>
    public void DeleteChoosingBulletList(ItemBulletButton chooseBulletDetail) {
        // アイテムバレット確定ボタンを非活性化
        btnConfirmItemBullet.interactable = false;

        // 登録リストから、選択したバレットを検索する
        foreach (ItemBulletButton chooseBullet in itemBulletsList) {
            if (chooseBullet.bulletData.bulletType == chooseBulletDetail.bulletData.bulletType) {

                // 選択可能なアイテムバレットをタップできる状態に戻す
                chooseBullet.SwitchItemBulletBtnInteractable(true);

                // アイテムバレットのステートを初期化
                chooseBullet.SetItemBulletStateType(ItemBulletButton.ItemBulletStateType.Unselected);
            }
        }

        // 登録リストから選択したアイテムバレットのゲームオブジェクトを破棄
        Destroy(chooseBulletDetail.gameObject);

        // 登録リストから選択したアイテムバレットのデータを削除
        choosingItemBulletsList.Remove(chooseBulletDetail);
    }

    /// <summary>
    /// 登録リスト内のアイテムバレットをすべて削除
    /// </summary>
    public void ResetChoosingBulletList() {
        // 削除対象がなければ処理しない
        if (choosingItemBulletsList.Count > 0) {
            return;
        }

        for (int i = 0; 0 < choosingItemBulletsList.Count; i++) {
            // 順番にアイテムバレットのゲームオブジェクトを破棄
            Destroy(choosingItemBulletsList[i]);
        }

        // 登録リストをクリア
        choosingItemBulletsList.Clear();
    }

    /// <summary>
    /// Selected 状態のアイテムバレットの状態を Unselected に戻す
    /// </summary>
    public void AllReturnItemBulletStateTypeToUnselected(ItemBulletButton.ItemBulletStateType newStateType) {
        for (int i = 0; i < itemBulletsList.Count; i++) {
            if (itemBulletsList[i].itemBulletStateType == ItemBulletButton.ItemBulletStateType.Selected) {

                // ItemBulletStateType を Unselected に戻す
                itemBulletsList[i].SetItemBulletStateType(newStateType);

                // フレームを非表示にする
                itemBulletsList[i].SwitchDisplayFrame(false);
            }
        }
    }

    /// <summary>
    /// アイテムバレット確定
    /// </summary>
    private void OnClickConfirmItemBullets() {
        // リスト作成
        List<BulletDataSO.BulletData> bulletDatas = new List<BulletDataSO.BulletData>();

        // 登録されたアイテムバレットからバレットデータのみを抽出
        for (int i = 0; i < choosingItemBulletsList.Count; i++) {
            bulletDatas.Add(choosingItemBulletsList[i].bulletData);
        }

        // バレットデータを登録
        gameManager.SetBulletDatas(bulletDatas);
    }

    /// <summary>
    /// ポップアップを閉じる(非表示)
    /// </summary>
    public void CloseChooseBulletPopUp() {

        // 徐々に見えなくする
        canvasGroupPopUp.DOFade(0, 1.0f);

        // ポップアップが見えなくなったら下の画面をタップできるようにする
        canvasGroupPopUp.blocksRaycasts = false;
    }
}
