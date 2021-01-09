using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BulletSelectDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnBulletSelect = null;

    [SerializeField]
    private Image imgBulletBtn = null;

    public BulletDataSO.BulletData bulletData; // Debug用に public

    private PlayerController playerController;

    private BulletSelectManager bulletSelectManager;

    private float launchTime;  // 発射できる時間

    private float initialLaunchTime;   // 発射できる時間の初期値

    private bool isLoading;    // 装填中かどうか。true なら装填中で発射できる。launchTime も減る

    public bool isDefaultBullet;  // 初期バレットかどうか。発射時間に関係なく発射できる

    [SerializeField]
    private Image imgLaunchTimeGauge;  // 発射できる時間のゲージ

    [SerializeField]
    private Text txtOpenExpValue;

    private bool isCostPayment;        // 現在コストを支払って開放済かどうか。trueならコスト支払い終了

    public bool isOpenAnimation;

    [SerializeField]
    private Image imgElementTypeBackground;

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
        imgBulletBtn.sprite = this.bulletData.btnSprite;

        // メソッド登録
        btnBulletSelect.onClick.AddListener(OnClickBulletSelect);

        SwitchActivateBulletBtn(false);

        // 発射時間設定
        initialLaunchTime = this.bulletData.launchTime;
        this.launchTime = initialLaunchTime;

        // ゲージを非表示にする
        imgLaunchTimeGauge.fillAmount = 0;

        // 開放可能なEXPを表示
        txtOpenExpValue.text = this.bulletData.openExp.ToString();

        // 初期バレット確認
        if (this.bulletData.openExp == 0) {
            // 初期バレット用の設定
            isDefaultBullet = true;
            isLoading = true;
            txtOpenExpValue.gameObject.SetActive(false);

            ChangeColorToBulletBtn(new Color(0.65f, 0.65f, 0.65f));

            bulletSelectManager.ChangeDefenseBaseElementType(this.bulletData.elementType);
        }

        // 背景画像の変更
        imgElementTypeBackground.sprite = DataBaseManager.instance.GetElementTypeSprite(this.bulletData.elementType);
    }

    /// <summary>
    /// バレット選択
    /// </summary>
    public void OnClickBulletSelect() {
        playerController.ChangeBullet(bulletData.bulletType);
        bulletSelectManager.ChangeLoadingBulletSettings(bulletData.bulletType);

        bulletSelectManager.ChangeDefenseBaseElementType(bulletData.elementType);

        // 重複防止
        if (!isDefaultBullet && imgLaunchTimeGauge.fillAmount == 0) {
            // ゲージを最大値にする
            imgLaunchTimeGauge.fillAmount = 1.0f;

            // コスト支払い済状態にする = 開放条件にてEXPが足りなくても押せない状態にならないようにする
            SetStateBulletCostPayment(true);

            Debug.Log(GetStateBulletCostPayment());

            // EXP減算
            bulletSelectManager.UpdateTotalExp(-bulletData.openExp);

            // コストEXP非表示
            //txtOpenExpValue.enabled = false;     
            txtOpenExpValue.gameObject.SetActive(false);
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
        imgBulletBtn.color = newColor;
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

            InitialzeBulletState();
        }
    }

    /// <summary>
    /// 初期バレット以外のバレットを初期状態に戻す
    /// </summary>
    private void InitialzeBulletState() {
        // 装填を止める
        isLoading = false;

        // 初期バレットに戻す
        bulletSelectManager.ActivateDefaultBullet();

        // 発射時間の初期化
        launchTime = initialLaunchTime;

        // 開放に必要なEXPを表示
        //txtOpenExpValue.enabled = true;
        txtOpenExpValue.gameObject.SetActive(true);

        // コスト未払いの状態に戻す
        SetStateBulletCostPayment(false);

        // 
        bulletSelectManager.JugdeOpenBullets();
    }

    /// <summary>
    /// コスト支払い状態の確認
    /// </summary>
    /// <returns></returns>
    public bool GetStateBulletCostPayment() {
        return isCostPayment;
    }

    /// <summary>
    /// コスト支払い状態の更新
    /// </summary>
    /// <param name="isSet"></param>
    public void SetStateBulletCostPayment(bool isSet) {
        isCostPayment = isSet;
    }

    /// <summary>
    /// コストが支払える状態になった際のアニメ演出 
    /// </summary>
    public void OpenBulletAnimation(bool isSet) {
        isOpenAnimation = isSet;
        if (isOpenAnimation) {
            transform.DOShakeScale(0.25f, 1, 10, 45).SetEase(Ease.Linear);
        }
    }
}
