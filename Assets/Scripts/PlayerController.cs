using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Bullet bulletPrefab;

    public BulletDataSO.BulletData bulletData;

    void Start()
    {

    }


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            tapPos.z = 2.0f;

            Vector3 direction = (tapPos - transform.position).normalized;
                    
            Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
        }    
    }
}
