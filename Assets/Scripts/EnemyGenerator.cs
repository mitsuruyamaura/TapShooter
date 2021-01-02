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

    void Start()
    {        
        //Generate();
    }

    void Update() {

        // �����I����ԂȂ珈�����Ȃ�
        if (isGenerateEnd) {
            return;
        }

        // �G�����̏���
        PreparateGenerateEnemy();               
    }

    /// <summary>
    /// �G�����̏���
    /// </summary>
    private void PreparateGenerateEnemy() {
        timer += Time.deltaTime;

        if (timer >= preparateTime) {
            timer = 0;
            GenerateEnemy();
        }
    }

    /// <summary>
    /// �G�̐���
    /// </summary>
    private void GenerateEnemy() {
        EnemyDataSO.EnemyData enemyData = GetEnemyData(EnemyDataSO.EnemyType.Normal);

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
    /// isGenerateEnd�̐؂�ւ�
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchGenerateState(bool isSwitch) {
        isGenerateEnd = isSwitch;
    }

    /// <summary>
    /// EnemyData�擾�p
    /// </summary>
    /// <param name="enemyType"></param>
    /// <returns></returns>
    private EnemyDataSO.EnemyData GetEnemyData(EnemyDataSO.EnemyType enemyType) {
        foreach (EnemyDataSO.EnemyData enemyData in DataBaseManager.instance.enemyDataSO.enemyDataList.Where(x => x.enemyType == enemyType)) {
            return enemyData;
        }
        return null;
    }

    /// <summary>
    /// BulletData�擾�p
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