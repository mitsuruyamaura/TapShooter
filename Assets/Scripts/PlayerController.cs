using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Bullet bulletPrefab;

    public BulletDataSO.BulletData bulletData;

    public int tapCount;

    private GameManager gameManager;

    public BulletDataSO.BulletType currentBulletType;

    [SerializeField]
    private CharaAnimationController charaAnim;


    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        bulletData = DataBaseManager.instance.GetPlayerBulletData(currentBulletType);
    }

    public void SetUpPlayer(GameManager gameManager) {  // <= Start の代わり
        this.gameManager = gameManager;
    }

    void Update() {

        //ShotDebug();


        // ゲームクリア、ゲームオーバーのいずれかなら
        if (gameManager.isGameUp) {
            return;
        }

        // 準備が整ってないなら
        if (!gameManager.isSetUpEnd) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            // 画面をタップ(クリック)した位置をカメラのスクリーン座標の情報を通じてワールド座標に変換
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 向きの生成(Z成分の除去と正規化)
            Vector3 direction = Vector3.Scale(tapPos - transform.position, new Vector3(1, 1, 0)).normalized;

            //Debug.Log(direction);

            // キャラの位置よりも下方向に向きがある場合(キャラの位置よりも下にある画面をタップした場合)
            if (direction.y <= 0.0f) {
                // 生成処理しない
                return;
            }

            // バレット生成
            GenerateBullet(direction);

            // 攻撃時のボイス再生
            SoundManager.instance.SetAttackVoice((SoundManager.AttackVoice)Random.Range(0, SoundManager.instance.AttackVoice_Clips.Length));

            // 攻撃アニメ再生
            charaAnim.PlayAnimation(CharaAnimationController.attackParameter);
        }
    }

    /// <summary>
    /// 弾を変更
    /// </summary>
    /// <param name="newBulletType"></param>
    public void ChangeBullet(BulletDataSO.BulletType newBulletType) {
        currentBulletType = newBulletType;

        bulletData = DataBaseManager.instance.GetPlayerBulletData(currentBulletType);
    }

    /// <summary>
    /// バレットの生成
    /// </summary>
    /// <param name="direction"></param>
    private void GenerateBullet(Vector3 direction) {
        switch (bulletData.bulletType) {
            case BulletDataSO.BulletType.Player_Normal:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                break;

            case BulletDataSO.BulletType.Player_Blaze:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                break;

            case BulletDataSO.BulletType.Player_3ways_Piercing:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x - 0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;

            case BulletDataSO.BulletType.Player_5ways_Normal:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x - 0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x - 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x + 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;
        }



        tapCount++;

        if (tapCount >= 50) {
            Debug.Log("バースト状態");
        }
    }

    private void Generate(Vector3 direction) {
        //Debug.Log("生成する位置情報 : " + direction);

        Bullet bulletObj = Instantiate(bulletPrefab, transform);
        //bulletObj.transform.localPosition = direction;

        //bulletObj.ShotBullet(transform.up);

        bulletObj.ShotBullet(direction);
    }

    private void ShotDebug() {
        if (Input.GetMouseButtonDown(0)) {
            //Instantiate(bulletPrefab, transform).ShotBullet();
            // 画面をタップ(クリック)した位置をカメラのスクリーン座標の情報を通じてワールド座標に変換
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Debug.Log("タップした位置情報 : " + tapPos);

            // 方向を計算
            Vector3 direction2 = tapPos - transform.position;

            //Debug.Log("方向 : " + direction2);

            // 方向の情報から、不要な Z成分(Z軸情報) の除去を行う
            //direction2 = Vector3.Scale(direction2, new Vector3(1, 1, 0));


            // 正規化処理を行い、単位ベクトルとする(方向の情報は持ちつつ、距離による速度差をなくして一定値にする)
            //direction2 = direction2.normalized;

            //Debug.Log("正規化処理後の方向 : " + direction2);

            Generate(direction2);
        }
    }
}
