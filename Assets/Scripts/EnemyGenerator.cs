using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab;

    [SerializeField]
    private Bullet bulletPrefab;

    //[SerializeField]
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

    //void Start()
    //{
    //    gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    //    Generate();
    //}

    public void SetUpEnemyGenerator(PlayerController playerController, GameManager gameManager) {
        this.playerController = playerController;
        this.gameManager = gameManager;
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
        EnemyDataSO.EnemyData enemyData = DataBaseManager.instance.GetEnemyData((EnemyDataSO.EnemyType)randomEnemyNo);

        if (enemyData == null) {
            return;
        }
            
        EnemyController enemy = Instantiate(enemyPrefab, transform);
        enemy.Inisialize(playerController, DataBaseManager.instance.GetEnemyBulletData(enemyData), enemyData, enemyData.bulletType == BulletDataSO.BulletType.None ? null : bulletPrefab);
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
    /// ƒ{ƒX¶¬
    /// </summary>
    public IEnumerator GenerateBoss() {

        yield return StartCoroutine(DisplayAlert());

        EnemyDataSO.EnemyData enemyData = DataBaseManager.instance.GetEnemyData(EnemyDataSO.EnemyType.Boss);
        EnemyController enemy = Instantiate(enemyPrefab, transform);
        enemy.Inisialize(playerController, DataBaseManager.instance.GetEnemyBulletData(enemyData), enemyData, enemyData.bulletType == BulletDataSO.BulletType.None ? null : bulletPrefab);
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
}
