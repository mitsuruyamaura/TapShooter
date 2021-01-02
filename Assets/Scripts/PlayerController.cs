using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Bullet bulletPrefab;

    public BulletDataSO.BulletData bulletData;

    public int tapCount;

    void Start()
    {

    }


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //tapPos.z = 2.0f;

            //Vector3 direction = (tapPos - transform.position).normalized;

            // 向きの生成（Z成分の除去と正規化）
            Vector3 direction = Vector3.Scale(tapPos - transform.position, new Vector3(1, 1, 0)).normalized;

            Instantiate(bulletPrefab, transform).Shot(bulletData, direction);

            tapCount++;

            if (tapCount >= 50) {
                Debug.Log("バースト状態");
            }
        }    
    }
}
