using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    public BulletDataSO.BulletData bulletData;

    public void Shot(BulletDataSO.BulletData bulletData, Vector3 direction) {
        this.bulletData = bulletData;

        GetComponent<Rigidbody2D>().AddForce(direction * bulletData.bulletSpeed);
    }
}
