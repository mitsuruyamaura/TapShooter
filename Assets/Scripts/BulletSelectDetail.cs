using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BulletSelectDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnBulletSelect;

    public Image imgBullet;

    public BulletDataSO.BulletData bulletData; // DebugópÇ… public

    private PlayerController playerController;

    private BulletSelectManager bulletSelectManager;

    /// <summary>
    /// èâä˙ê›íË
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
    /// ëIë
    /// </summary>
    private void OnClickBulletSelect() {
        playerController.ChangeBullet(bulletData.bulletType);
        bulletSelectManager.ChangeColorToBulletButton(bulletData.bulletType);
    }
}
