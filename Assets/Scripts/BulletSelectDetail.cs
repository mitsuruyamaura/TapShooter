using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BulletSelectDetail : MonoBehaviour
{
    public Button btnBulletSelect;

    public Image imgBullet;

    public BulletDataSO.BulletData bulletData; // Debug用に public

    private PlayerController playerController;

    private BulletSelectManager bulletSelectManager;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="bulletData"></param>
    /// <param name="playerController"></param>
    /// <param name="bulletSelectManager"></param>
    public void SetUpBulletSelectDetail(BulletDataSO.BulletData bulletData, PlayerController playerController, BulletSelectManager bulletSelectManager) {
        this.bulletData = bulletData;
        this.playerController = playerController;
        this.bulletSelectManager = bulletSelectManager;

        imgBullet.sprite = bulletData.sprite;

        btnBulletSelect.onClick.AddListener(OnClickBulletSelect);
    }

    /// <summary>
    /// 選択
    /// </summary>
    private void OnClickBulletSelect() {
        playerController.ChangeBullet(bulletData.bulletType);
        bulletSelectManager.ChangeColorToBulletButton(bulletData.bulletType);
    }

    /// <summary>
    /// バレットをタップ可否の更新
    /// </summary>
    public void SwitchActivateBulletBtn(bool isSwitch) {
        btnBulletSelect.interactable = isSwitch;
    }
}
