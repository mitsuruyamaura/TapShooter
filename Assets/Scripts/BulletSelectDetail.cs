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

    private float launchTime;  // 発射できる時間

    private float initialLaunchTime;   // 発射できる時間の初期値

    private bool isLoading;    // 装填中かどうか。true なら装填中で発射できる。launchTime も減る

    public bool isDefaultBullet;  // 初期バレットかどうか。発射時間に関係なく発射できる

    [SerializeField]
    private Image imgLaunchTimeGauge;  // 発射できる時間のゲージ

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

        // 画像変更
        imgBullet.sprite = this.bulletData.sprite;

        // メソッド登録
        btnBulletSelect.onClick.AddListener(OnClickBulletSelect);

        // 発射時間設定
        initialLaunchTime = this.bulletData.launchTime;
        this.launchTime = initialLaunchTime;

        // ゲージを非表示にする
        imgLaunchTimeGauge.fillAmount = 0;

        // 初期バレット確認
        if (this.bulletData.openExp == 0) {
            // 初期バレット用の設定
            isDefaultBullet = true;
            isLoading = true;
            ChangeColorToBulletBtn(new Color(0.65f, 0.65f, 0.65f));
        }
    }

    /// <summary>
    /// バレット選択
    /// </summary>
    public void OnClickBulletSelect() {
        playerController.ChangeBullet(bulletData.bulletType);
        bulletSelectManager.ChangeLoadingBulletSettings(bulletData.bulletType);

        // 重複防止
        if (!isDefaultBullet && imgLaunchTimeGauge.fillAmount == 0) {
            // ゲージを最大値にする
            imgLaunchTimeGauge.fillAmount = 1.0f;
        }
    }

    /// <summary>
    /// 装填中のバレットを切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ChangeLoadingBullet(bool isSwitch) {
        isLoading = isSwitch;
    }

    /// <summary>
    /// バレットをタップ可否の更新
    /// </summary>
    public void SwitchActivateBulletBtn(bool isSwitch) {
        btnBulletSelect.interactable = isSwitch;
    }

    /// <summary>
    /// ボタンの色を変更
    /// </summary>
    /// <param name="newColor"></param>
    public void ChangeColorToBulletBtn(Color newColor) {
        imgBullet.color = newColor;
    }

    void Update() {

        // 初期バレットは発射できる時間制限なし
        if (isDefaultBullet) {
            return;
        }

        // 装填しているバレットでなければ何もしない
        if (!isLoading) {
            return;
        }

        // 発射できる時間を減らす
        launchTime -= Time.deltaTime;

        // ゲージを合わせる
        imgLaunchTimeGauge.DOFillAmount(launchTime / initialLaunchTime, 0.25f);

        // 発射できる時間がなくなったら
        if (launchTime <= 0) {
            launchTime = 0;

            // 装填を止める
            isLoading = false;

            // 初期バレットに戻す
            bulletSelectManager.ActivateDefaultBullet();

            launchTime = initialLaunchTime;
        }
    }
}
