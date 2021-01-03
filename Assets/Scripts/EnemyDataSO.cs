using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Create EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public List<EnemyData> enemyDataList = new List<EnemyData>();

    [Serializable]
    public class EnemyData {
        public int no;
        public EnemyType enemyType;
        public int hp;
        public int power;
        public BulletDataSO.BulletType bulletType;
        public Sprite enemySprite; 
    }

    [Serializable]
    public enum EnemyType {
        Normal_0,
        Normal_1,
        Normal_2,
        Elite_0,
        Elite_1,
        Elite_2,
        Boss
    }
}
