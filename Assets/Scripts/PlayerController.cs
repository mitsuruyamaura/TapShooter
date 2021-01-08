using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Bullet bulletPrefab;

    public BulletDataSO.BulletData bulletData;

    public int tapCount;

    private GameManager gameManager;

    public BulletDataSO.BulletType currentBulletType;



    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        bulletData = DataBaseManager.instance.GetPlayerBulletData(currentBulletType);
    }


    void Update() {
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

    private void GenerateBullet(Vector3 direction) {
        switch (bulletData.bulletType) {
            case BulletDataSO.BulletType.D:
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                break;
            case BulletDataSO.BulletType.E:
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                break;
            case BulletDataSO.BulletType.F:
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x -0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;
            case BulletDataSO.BulletType.G:
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x - 0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x - 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x + 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;
        }



        tapCount++;

        if (tapCount >= 50) {
            Debug.Log("バースト状態");
        }
    }
}
