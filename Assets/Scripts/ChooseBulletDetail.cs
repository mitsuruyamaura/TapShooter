using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChooseBulletDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnChooseBullet;

    [SerializeField]
    private Image imgBulletBtn;               // バレット画像

    [SerializeField]
    private Image imgElementTypeBackground;   // 属性画像

    [SerializeField]
    private Text txtOpenExpValue;             // EXP表示

    [SerializeField]
    private Image imgFrame;                   // ボタンのフレーム

    public enum DetailStateType {
        Unselected,   // 未選択
        Selected,     // 選択中
        Choosing,     // セットして使用できる状態
    }
    public DetailStateType stateType;

    private ChooseBulletPopUp chooseBulletPopUp;

    public BulletDataSO.BulletData bulletData;

    /// <summary>
    /// バレット選択の設定
    /// </summary>
    /// <param name="chooseBulletPopUp"></param>
    public void SetUpChooseBullet(ChooseBulletPopUp chooseBulletPopUp, DetailStateType detailStateType, BulletDataSO.BulletData bulletData) {
        this.chooseBulletPopUp = chooseBulletPopUp;

        btnChooseBullet.onClick.AddListener(OnClickChooseBullet);

        // 
        stateType = detailStateType;

        this.bulletData = bulletData;

        imgBulletBtn.sprite = this.bulletData.btnSprite;

        // 属性を足す
        imgElementTypeBackground.sprite = DataBaseManager.instance.GetElementTypeSprite(this.bulletData.elementType);

        // EXP表示を足す
        txtOpenExpValue.text = this.bulletData.openExp.ToString();

        // 選択中フレームを隠す
        SwitchFrame(false);
    }

    /// <summary>
    /// バレットをタップした際の処理
    /// </summary>
    private void OnClickChooseBullet() {

        // 未選択
        if (stateType == DetailStateType.Unselected) {

            // すべてのバレットのボタンを確認して、Selected を Unselected にする
            chooseBulletPopUp.ResetStateTypeAllBtn(DetailStateType.Unselected);

            // 選択中
            SetState(DetailStateType.Selected);

            // 説明表示
            chooseBulletPopUp.UpdateDisplayBulletPerformance(bulletData);

            // ボタンの背景の色を変えるか、フレームに色を付ける
            SwitchFrame(true);



        // 選択中
        } else if (stateType == DetailStateType.Selected){

            SetState(DetailStateType.Choosing);

            chooseBulletPopUp.AddChooseBulletDetail(this);

            ActivateBtn(false);

            SwitchFrame(false);

        // 登録中
        } else if (stateType == DetailStateType.Choosing) {
            chooseBulletPopUp.DeleteChoosingBulletList(this);
        }
    }

    /// <summary>
    /// ボタンの活性化・非活性化
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ActivateBtn(bool isSwitch) {
        btnChooseBullet.interactable = isSwitch;
    }

    /// <summary>
    /// StateTypeの設定
    /// </summary>
    public void SetState(DetailStateType newStateType) {
        stateType = newStateType;
    }

    public void SwitchFrame(bool isSwitch) {
        imgFrame.enabled = isSwitch;
    }
}
