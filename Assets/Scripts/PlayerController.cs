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
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 向きの生成（Z成分の除去と正規化）
            Vector3 direction = Vector3.Scale(tapPos - transform.position, new Vector3(1, 1, 0)).normalized;

            Instantiate(bulletPrefab, transform).Shot(bulletData, direction);

            tapCount++;

            if (tapCount >= 50) {
                Debug.Log("バースト状態");
            }
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
}
