using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private EnemyGenerator enemyGenerator;

    [SerializeField]
    private BulletSelectManager bulletSelectManager;

    [SerializeField]
    private PlayerController playerController;

    private bool isSetUpEnd;

    public bool isGameUp;

    public int waveCount = 0;

    public int maxWaveCount;

    IEnumerator Start()
    {
        yield return StartCoroutine(bulletSelectManager.GenerateBulletSelectDetail(playerController));

        StartCoroutine(ObservateGenerateEnemyState());
    }

    /// <summary>
    /// 敵の生成を監視
    /// </summary>
    /// <returns></returns>
    private IEnumerator ObservateGenerateEnemyState() {
        Debug.Log("監視開始");
        bool isAllGenerate = false;

        while (!isAllGenerate) {
            if (enemyGenerator.isGenerateEnd) {
                isAllGenerate = true;
            }
            yield return null;
        }
        Debug.Log("監視終了");

        if (waveCount >= maxWaveCount) {
            Debug.Log("Game Clear");
        } else {
            AdvanceWave();
        }   
    }

    private IEnumerator ObservateBossState() {
        Debug.Log("ボス監視開始");

        while (!enemyGenerator.isBossDestroyed) {
            yield return null;
        }

        Debug.Log("ゲームクリア");

        // エネミーをすべて削除
        enemyGenerator.ClearEnemyList();

        // クリア表示

    }

    /// <summary>
    /// Wave進行
    /// </summary>
    private void AdvanceWave() {
        waveCount++;
        Debug.Log(waveCount);
        if (waveCount == maxWaveCount - 1) {
            Debug.Log("Boss");

            // ボス生成
            StartCoroutine(enemyGenerator.GenerateBoss());

            // ボス監視
            StartCoroutine(ObservateBossState());

        } else {
            // 生成再開
            enemyGenerator.SwitchGenerateState(false);

            // 監視再開
            StartCoroutine(ObservateGenerateEnemyState());
        }
    }

    /// <summary>
    /// ゲーム終了状態を切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchGameUp(bool isSwitch) {
        isGameUp = isSwitch;

        if (isGameUp) {
            enemyGenerator.ClearEnemyList();
        }
    }

    void Update()
    {
        
    }
}
