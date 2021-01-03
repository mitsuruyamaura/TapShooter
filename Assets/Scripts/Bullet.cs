using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    public BulletDataSO.BulletData bulletData;

    [SerializeField]
    private Image imgBullet;

    public void Shot(BulletDataSO.BulletData bulletData, Vector3 direction) {
        this.bulletData = bulletData;
        imgBullet.sprite = bulletData.sprite;

        if (bulletData.liberalType == BulletDataSO.LiberalType.Player) {
            transform.eulerAngles = new Vector3(0, 0, 180);
            transform.localScale = new Vector3(2, 2, 2);
        }
        GetComponent<Rigidbody2D>().AddForce(direction * bulletData.bulletSpeed);
    }
}
