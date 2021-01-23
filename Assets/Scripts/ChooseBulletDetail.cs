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
    private Image imgBullet;

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

        imgBullet.sprite = this.bulletData.btnSprite;

        // 属性を足す

        // EXP表示を足す

    }

    /// <summary>
    /// バレットをタップした際の処理
    /// </summary>
    private void OnClickChooseBullet() {

        // 未選択
        if (stateType == DetailStateType.Unselected) {

            // 選択中
            SetState(DetailStateType.Selected);

            chooseBulletPopUp.UpdateDisplayBulletPerformance(bulletData);

        // 選択中
        } else if (stateType == DetailStateType.Selected){

            SetState(DetailStateType.Choosing);

            chooseBulletPopUp.AddChooseBulletDetail(this);

            ActivateBtn(false);

        // 
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
}
