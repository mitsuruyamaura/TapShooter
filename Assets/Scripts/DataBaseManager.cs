using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public MoveDataSO moveDataSO;

    public EnemyDataSO enemyDataSO;

    public BulletDataSO bulletDataSO;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// EnemyDataŽæ“¾—p
    /// </summary>
    /// <param name="enemyType"></param>
    /// <returns></returns>
    public EnemyDataSO.EnemyData GetEnemyData(EnemyDataSO.EnemyType enemyType) {
        Debug.Log(enemyType);
        foreach (EnemyDataSO.EnemyData enemyData in enemyDataSO.enemyDataList.Where(x => x.enemyType == enemyType)) {
            return enemyData;
        }
        return null;
    }

    /// <summary>
    /// BulletDataŽæ“¾—p
    /// </summary>
    /// <param name="enemyData"></param>
    /// <returns></returns>
    public BulletDataSO.BulletData GetEnemyBulletData(EnemyDataSO.EnemyData enemyData) {
        foreach (BulletDataSO.BulletData bulletData in bulletDataSO.bulletDataList.Where((x) => x.bulletType == enemyData.bulletType)) {
            return bulletData;
        }
        return null;
    }

    public BulletDataSO.BulletData GetPlayerBulletData(BulletDataSO.BulletType bulletType) {
        foreach (BulletDataSO.BulletData bulletData in bulletDataSO.bulletDataList.Where((x) => x.bulletType == bulletType)) {
            return bulletData;
        }
        return bulletDataSO.bulletDataList[0];
    }
}
