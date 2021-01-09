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
        public Sprite btnSprite;
        public LiberalType liberalType;
        public int openExp;
        public ElementType elementType;
        public float launchTime;
        public Sprite bulletSprite;
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
        Enemy,
        BOSS
    }

    [Serializable]
    public class Element {
        public int no;
        public Sprite elementSprite;
    }

    public List<Element> elementList = new List<Element>();
}
