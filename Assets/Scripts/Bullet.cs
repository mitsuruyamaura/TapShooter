using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    public BulletDataSO.BulletData bulletData;

    [SerializeField]
    private Image imgBullet;

    private bool isTarget;

    private Vector3 nearPos;

    /// <summary>
    /// バレットの制御
    /// </summary>
    /// <param name="bulletData"></param>
    /// <param name="direction"></param>
    public void Shot(BulletDataSO.BulletData bulletData, Vector3 direction) {

        // 追尾弾のみ処理する
        if (bulletData.bulletType == BulletDataSO.BulletType.Player_Blaze) {

            // 発射時にゲーム内にいるエネミーの情報を取得
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // エネミーが１体以上いるなら
            if (enemies.Length > 0) {

                // 一番近いエネミーとして、配列の最初のエネミーの位置情報を仮に登録
                nearPos = enemies[0].transform.position;

                // すべてのエネミーの位置を順番に確認
                for (int i = 0; i < enemies.Length; i++) {
                    // これは省略可能
                    Vector3 pos = enemies[i].transform.position;
                    // 現在のエネミーの位置と比較して、画面の下に近いものを nearPos として更新する
                    if (nearPos.x < pos.x && nearPos.y < pos.y) {
                        nearPos = pos;
                        //Debug.Log(nearPos);
                    }
                }

                // 追尾するターゲットありとして登録し、Updateメソッドを動くようにする
                isTarget = true;
            }

        }

        this.bulletData = bulletData;
        imgBullet.sprite = bulletData.bulletSprite;

        if (bulletData.liberalType == BulletDataSO.LiberalType.Player) {
            //transform.eulerAngles = new Vector3(0, 0, 180);
            transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        }

        if (bulletData.liberalType == BulletDataSO.LiberalType.BOSS) {
            //transform.SetParent(GameObject.FindGameObjectWithTag("BulletPool").transform);
            transform.SetParent(DataBaseManager.instance.GetTemporaryObjectContainerTransform());
        }

        // 追尾弾以外
        if (bulletData.bulletType != BulletDataSO.BulletType.Player_Blaze) {
            GetComponent<Rigidbody2D>().AddForce(direction * bulletData.bulletSpeed);
        } 
        //else {
            //dir = Vector3.Scale(nearPos - transform.position, new Vector3(1, 1, 0)).normalized;
            //rad = Mathf.Atan2(dir.y, dir.x);

            //Debug.Log(rad);
        //}

        Destroy(gameObject, 5.0f);
    }

    private void Update() {
        if (!isTarget) {
            return;
        }

        // 追尾用(タップ位置に関係なく、キャラに近いエネミーに発射する)
        Vector3 currentPos = transform.position;
        Vector3 targetPos = nearPos;

        transform.position = Vector3.MoveTowards(currentPos, targetPos, Time.deltaTime / 10 * bulletData.bulletSpeed);

        //// 現在位置をPositionに代入
        //Position = transform.localPosition;
        //// x += SPEED * cos(ラジアン)
        //// y += SPEED * sin(ラジアン)
        //// これで特定の方向へ向かって進んでいく。
        //Position.x += bulletData.bulletSpeed * Mathf.Cos(rad);
        //Position.y += bulletData.bulletSpeed * Mathf.Sin(rad);
        //// 現在の位置に加算減算を行ったPositionを代入する
        //transform.localPosition = Position;
    }
}
