using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BulletDataSO", menuName = "Create BulletDataSO")]
public class BulletDataSO : ScriptableObject {

    public List<BulletData> bulletDataList = new List<BulletData>();

    [Serializable]
    public class BulletData {
        public float bulletSpeed;
        public int bulletPower;
        public float loadingTime;
        public BulletType bulletType;
        public Sprite sprite;
        public LiberalType liberalType;
    }

    [Serializable]
    public enum BulletType {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        None
    }

    [Serializable]
    public enum LiberalType {
        Player,
        Enemy
    }
}
