using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

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

    public bool isBossDestroyed;

    [SerializeField]
    private CanvasGroup canvasGroupBossAlert;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //Generate();
    }

    void Update() {

        if (gameManager.isGameUp) {
            return;
        }

        // 生成終了状態なら処理しない
        if (isGenerateEnd) {
            return;
        }

        // 敵生成の準備
        PreparateGenerateEnemy();               
    }

    /// <summary>
    /// 敵生成の準備
    /// </summary>
    private void PreparateGenerateEnemy() {
        timer += Time.deltaTime;

        if (timer >= preparateTime) {
            timer = 0;
            GenerateEnemy();
        }
    }

    /// <summary>
    /// 敵の生成
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
    /// isGenerateEndの切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchGenerateState(bool isSwitch) {
        isGenerateEnd = isSwitch;
    }

    /// <summary>
    /// EnemyListをクリア
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
    /// ボス生成
    /// </summary>
    public IEnumerator GenerateBoss() {

        yield return StartCoroutine(DisplayAlert());

        EnemyDataSO.EnemyData enemyData = GetEnemyData(EnemyDataSO.EnemyType.Boss);
        EnemyController enemy = Instantiate(enemyPrefab, transform);
        enemy.Inisialize(playerController, GetBulletData(enemyData), enemyData, enemyData.bulletType == BulletDataSO.BulletType.None ? null : bulletPrefab);
        enemy.InitializeBoss(this);
        enemyList.Add(enemy);

        generateCount++;

        if (generateCount >= maxGenerateCount) {
            isGenerateEnd = true;
            generateCount = 0;
        }
    }

    private IEnumerator DisplayAlert() {
        canvasGroupBossAlert.transform.parent.gameObject.SetActive(true);
        canvasGroupBossAlert.DOFade(1.0f, 0.5f).SetLoops(6, LoopType.Yoyo);
        yield return new WaitForSeconds(3.0f);
        canvasGroupBossAlert.DOFade(0.0f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        canvasGroupBossAlert.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// EnemyData取得用
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
    /// BulletData取得用
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
