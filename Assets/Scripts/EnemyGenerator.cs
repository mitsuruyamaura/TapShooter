using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab;

    [SerializeField]
    private Bullet bulletPrefab;

    [SerializeField]
    private PlayerController playerController;

    public List<EnemyController> enemyList = new List<EnemyController>();

    public int generateCount;

    public int maxGenerateCount;

    public bool isGenerateEnd;

    private float timer;

    public int preparateTime;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //Generate();
    }

    void Update() {

        if (gameManager.isGameUp) {
            return;
        }

        // ¶¬I—¹ó‘Ô‚È‚çˆ—‚µ‚È‚¢
        if (isGenerateEnd) {
            return;
        }

        // “G¶¬‚Ì€”õ
        PreparateGenerateEnemy();               
    }

    /// <summary>
    /// “G¶¬‚Ì€”õ
    /// </summary>
    private void PreparateGenerateEnemy() {
        timer += Time.deltaTime;

        if (timer >= preparateTime) {
            timer = 0;
            GenerateEnemy();
        }
    }

    /// <summary>
    /// “G‚Ì¶¬
    /// </summary>
    private void GenerateEnemy() {
        int randomEnemyNo = Random.Range(0, DataBaseManager.instance.enemyDataSO.enemyDataList.Count);
        EnemyDataSO.EnemyData enemyData = GetEnemyData((EnemyDataSO.EnemyType)randomEnemyNo);

        if (enemyData == null) {
            return;
        }
            
        EnemyController enemy = Instantiate(enemyPrefab, transform);
        enemy.Inisialize(playerController, GetBulletData(enemyData), enemyData, enemyData.bulletType == BulletDataSO.BulletType.None ? null : bulletPrefab);
        enemyList.Add(enemy);

        generateCount++;

        if (generateCount >= maxGenerateCount) {
            isGenerateEnd = true;
            generateCount = 0;
        }
    }

    /// <summary>
    /// isGenerateEnd‚ÌØ‚è‘Ö‚¦
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchGenerateState(bool isSwitch) {
        isGenerateEnd = isSwitch;
    }

    /// <summary>
    /// EnemyList‚ğƒNƒŠƒA
    /// </summary>
    public void ClearEnemyList() {
        for (int i = 0; i < enemyList.Count; i++) {
            if (enemyList[i] != null) {
                Destroy(enemyList[i].gameObject);
            }
        }
        enemyList.Clear();
    }

    /// <summary>
    /// EnemyDataæ“¾—p
    /// </summary>
    /// <param name="enemyType"></param>
    /// <returns></returns>
    private EnemyDataSO.EnemyData GetEnemyData(EnemyDataSO.EnemyType enemyType) {
        Debug.Log(enemyType);
        foreach (EnemyDataSO.EnemyData enemyData in DataBaseManager.instance.enemyDataSO.enemyDataList.Where(x => x.enemyType == enemyType)) {
            return enemyData;
        }
        return null;
    }

    /// <summary>
    /// BulletDataæ“¾—p
    /// </summary>
    /// <param name="enemyData"></param>
    /// <returns></returns>
    private BulletDataSO.BulletData GetBulletData(EnemyDataSO.EnemyData enemyData) {
        foreach (BulletDataSO.BulletData bulletData in DataBaseManager.instance.bulletDataSO.bulletDataList.Where((x) => x.bulletType == enemyData.bulletType)) {
            return bulletData;
        }
        return null;
    }
}
