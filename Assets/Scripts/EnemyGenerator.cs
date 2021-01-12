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

    public float preparateTime;

    private GameManager gameManager;

    public bool isBossDestroyed;

    [SerializeField]
    private CanvasGroup canvasGroupBossAlert;

    public BulletSelectManager bulletSelectManager;

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

        if (!gameManager.isSetUpEnd) {
            return;
        }

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
        int randomEnemyNo = Random.Range(0, DataBaseManager.instance.enemyDataSO.enemyDataList.Count);
        EnemyDataSO.EnemyData enemyData = DataBaseManager.instance.GetEnemyData((EnemyDataSO.EnemyType)randomEnemyNo);

        if (enemyData == null) {
            return;
        }
            
        EnemyController enemy = Instantiate(enemyPrefab, transform);
        enemy.Inisialize(playerController, DataBaseManager.instance.GetEnemyBulletData(enemyData), enemyData, enemyData.bulletType == BulletDataSO.BulletType.None ? null : bulletPrefab);
        enemy.AdditionalInitialize(this);
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
    /// EnemyList���N���A
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
    /// �{�X����
    /// </summary>
    public IEnumerator GenerateBoss() {

        yield return StartCoroutine(DisplayAlert());

        EnemyDataSO.EnemyData enemyData = DataBaseManager.instance.GetEnemyData(EnemyDataSO.EnemyType.Boss);
        EnemyController enemy = Instantiate(enemyPrefab, transform);
        enemy.Inisialize(playerController, DataBaseManager.instance.GetEnemyBulletData(enemyData), enemyData, enemyData.bulletType == BulletDataSO.BulletType.None ? null : bulletPrefab);
        enemy.AdditionalInitialize(this);
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
    /// EXP���Z
    /// </summary>
    /// <param name="exp"></param>
    public void UpdateExp(int exp) {
        bulletSelectManager.UpdateTotalExp(exp, BulletSelectManager.ExpType.EnemyExp);
    }

    /// <summary>
    /// �ꎞ�I�u�W�F�N�g(�o���b�g�A�G�t�F�N�g�A��)�����ׂĔj��
    /// </summary>
    public void DestroyTemporaryObjectContainer() {
        //Destroy(GameObject.FindGameObjectWithTag("BulletPool"));
        Destroy(DataBaseManager.instance.GetTemporaryObjectContainerTransform().gameObject);
    }

    /// <summary>
    /// �󔠂̊l�����̍X�V��GameManager�ɒʒm
    /// </summary>
    public void NoticeTeasureBoxCountToGameManager() {
        gameManager.UpdateTreasureBoxCount();
    }
}
