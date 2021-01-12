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

    private Transform temporaryObjectContainerTran;

    private Transform treasureBoxIconTran;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize() {
        // TODO 何かあれば追加

        // シーンの開始に取得(シングルトンクラスであるため、再スタートした場合にも取得する)
        temporaryObjectContainerTran = GameObject.FindGameObjectWithTag("TemporaryObjectContainer").transform;

        treasureBoxIconTran = GameObject.FindGameObjectWithTag("TreasureBox").transform;
    }

    /// <summary>
    /// EnemyData取得用
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
    /// EnemyDataからBulletData取得用
    /// </summary>
    /// <param name="enemyData"></param>
    /// <returns></returns>
    public BulletDataSO.BulletData GetEnemyBulletData(EnemyDataSO.EnemyData enemyData) {
        foreach (BulletDataSO.BulletData bulletData in bulletDataSO.bulletDataList.Where((x) => x.bulletType == enemyData.bulletType)) {
            return bulletData;
        }
        return null;
    }

    /// <summary>
    /// BulletTypeからBulletData取得用
    /// </summary>
    /// <param name="bulletType"></param>
    /// <returns></returns>
    public BulletDataSO.BulletData GetPlayerBulletData(BulletDataSO.BulletType bulletType) {
        foreach (BulletDataSO.BulletData bulletData in bulletDataSO.bulletDataList.Where((x) => x.bulletType == bulletType)) {
            return bulletData;
        }
        return bulletDataSO.bulletDataList[0];
    }

    /// <summary>
    /// ElementTypeからSprite取得用
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    public Sprite GetElementTypeSprite(ElementType elementType) {
        foreach(BulletDataSO.Element element in bulletDataSO.elementList.Where(x => x.no == (int)elementType)) {
            return element.elementSprite;
        }
        return bulletDataSO.elementList[0].elementSprite;
    }

    /// <summary>
    /// 宝箱アイコンの位置情報の取得
    /// </summary>
    /// <returns></returns>
    public Transform GetTresureBoxIconTransfrom() {
        return treasureBoxIconTran;
    }

    /// <summary>
    /// 一時オブジェクト(バレット、エフェクト、宝箱など)の格納用ゲームオブジェクトの位置情報の取得
    /// </summary>
    /// <returns></returns>
    public Transform GetTemporaryObjectContainerTransform() {
        return temporaryObjectContainerTran;
    }
}
