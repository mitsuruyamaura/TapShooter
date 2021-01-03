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
        public EnemyType enemyType;
        public int hp;
        public int power;
        public BulletDataSO.BulletType bulletType;
    }

    [Serializable]
    public enum EnemyType {
        Normal,
        Elite,
        Boss
    }
}
