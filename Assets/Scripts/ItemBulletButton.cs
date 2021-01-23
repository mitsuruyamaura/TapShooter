using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBulletButton : MonoBehaviour
{
    [SerializeField]
    private Button btnItemBullet;

    [SerializeField]
    private Image imgBullet;                  // バレット画像

    [SerializeField]
    private Image imgElementTypeBackground;   // 属性画像

    [SerializeField]
    private Text txtOpenExpValue;             // EXP表示

    [SerializeField]
    private Image imgFrame;                   // ボタンのフレーム

    public enum ItemBulletStateType {
        Unselected,   // 未選択
        Selected,     // 選択中
        Choosing,     // セットして使用できる状態
    }

    [Header("現在のアイテムバレットの状態")]
    public ItemBulletStateType itemBulletStateType;

    private ChooseBulletPopUp chooseBulletPopUp;

    public BulletDataSO.BulletData bulletData;

    /// <summary>
    /// アイテムバレットの設定
    /// </summary>
    /// <param name="chooseBulletPopUp"></param>
    /// <param name="detailStateType"></param>
    /// <param name="bulletData"></param>
    public void SetUpChooseBullet(ChooseBulletPopUp chooseBulletPopUp, ItemBulletStateType detailStateType, BulletDataSO.BulletData bulletData) {
        this.chooseBulletPopUp = chooseBulletPopUp;
        this.bulletData = bulletData;

        // ボタンにメソッドを登録
        btnItemBullet.onClick.AddListener(OnClickItemBullet);

        // 現在のタップの状態を設定
        SetItemBulletStateType(detailStateType);

        // 画像差し替え
        imgBullet.sprite = this.bulletData.btnSprite;

        // 属性属性差し替え
        imgElementTypeBackground.sprite = DataBaseManager.instance.GetElementTypeSprite(this.bulletData.elementType);

        // EXP表示差し替え
        txtOpenExpValue.text = this.bulletData.openExp.ToString();

        // 選択中フレームを隠す
        SwitchDisplayFrame(false);
    }

    /// <summary>
    /// アイテムバレットをタップした際の処理
    /// </summary>
    private void OnClickItemBullet() {
        switch (itemBulletStateType) {
            // 未選択
            case ItemBulletStateType.Unselected:
                // すべてのバレットのボタンを確認して、Selected を Unselected にする
                chooseBulletPopUp.AllReturnItemBulletStateTypeToUnselected(ItemBulletStateType.Unselected);

                // 選択中に変更
                SetItemBulletStateType(ItemBulletStateType.Selected);

                // バレットの説明表示
                chooseBulletPopUp.UpdateDisplayBulletPerformance(bulletData);

                // 選択中のフレームを表示
                SwitchDisplayFrame(true);
                break;

            // 選択中
            case ItemBulletStateType.Selected:

                // 登録中に変更
                SetItemBulletStateType(ItemBulletStateType.Choosing);

                // 登録バレットに表示
                chooseBulletPopUp.AddChooseBulletDetail(this);

                // ボタンを押せないようにする
                SwitchItemBulletBtnInteractable(false);

                // 選択中のフレームを非表示にする
                SwitchDisplayFrame(false);
                break;

            // 登録中
            case ItemBulletStateType.Choosing:

                // 登録を解除
                chooseBulletPopUp.DeleteChoosingBulletList(this);
                break;
        }
    }

    /// <summary>
    /// アイテムバレットボタンの活性化・非活性化
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchItemBulletBtnInteractable(bool isSwitch) {
        btnItemBullet.interactable = isSwitch;
    }

    /// <summary>
    /// ItemBulletStateTypeのセット
    /// </summary>
    /// <param name="newStateType"></param>
    public void SetItemBulletStateType(ItemBulletStateType newStateType) {
        itemBulletStateType = newStateType;
    }

    /// <summary>
    /// フレームの表示オンオフの切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchDisplayFrame(bool isSwitch) {
        imgFrame.enabled = isSwitch;
    }
}
