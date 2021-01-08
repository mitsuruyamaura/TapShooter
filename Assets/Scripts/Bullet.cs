using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    public BulletDataSO.BulletData bulletData;

    [SerializeField]
    private Image imgBullet;

    bool isTarget;

    Vector3 Position;

    float rad;

    public Vector3 nearPos;

    Vector3 dir;

    public void Shot(BulletDataSO.BulletData bulletData, Vector3 direction) {

        nearPos = Vector3.zero;
        if (bulletData.bulletType == BulletDataSO.BulletType.E) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length > 0) {
                nearPos = enemies[0].transform.position;
                Vector3 myPos = transform.position;
                for (int i = 0; i < enemies.Length; i++) {
                    Vector3 pos = enemies[i].transform.position - myPos;

                    if (nearPos.x < pos.x && nearPos.y < pos.y) {
                        nearPos = pos;
                    }
                }
            }
            Debug.Log(nearPos);
            isTarget = true;
        }

        this.bulletData = bulletData;
        imgBullet.sprite = bulletData.sprite;

        if (bulletData.liberalType == BulletDataSO.LiberalType.Player) {
            transform.eulerAngles = new Vector3(0, 0, 180);
            transform.localScale = new Vector3(2, 2, 2);
        }

        if (bulletData.bulletType != BulletDataSO.BulletType.E) {
            GetComponent<Rigidbody2D>().AddForce(direction * bulletData.bulletSpeed);
        } else {
            dir = Vector3.Scale(nearPos - transform.position, new Vector3(1, 1, 0)).normalized;
            rad = Mathf.Atan2(dir.y, dir.x);

            Debug.Log(rad);
        }

        Destroy(gameObject, 5.0f);
    }

    private void Update() {
        if (!isTarget) {
            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = nearPos;

        transform.position = Vector3.MoveTowards(currentPos, targetPos, Time.deltaTime * bulletData.bulletSpeed);

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
