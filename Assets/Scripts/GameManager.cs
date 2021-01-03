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
    /// GÌ¶¬ðÄ
    /// </summary>
    /// <returns></returns>
    private IEnumerator ObservateGenerateEnemyState() {
        Debug.Log("ÄJn");
        bool isAllGenerate = false;

        while (!isAllGenerate) {
            if (enemyGenerator.isGenerateEnd) {
                isAllGenerate = true;
            }
            yield return null;
        }
        Debug.Log("ÄI¹");

        if (waveCount >= maxWaveCount) {
            Debug.Log("Game Clear");
        } else {
            AdvanceWave();
        }   
    }

    private IEnumerator ObservateBossState() {
        Debug.Log("{XÄJn");

        while (!enemyGenerator.isBossDestroyed) {
            yield return null;
        }

        Debug.Log("Q[NA");

        // Gl~[ð·×Äí
        enemyGenerator.ClearEnemyList();

        // NA\¦

    }

    /// <summary>
    /// Waveis
    /// </summary>
    private void AdvanceWave() {
        waveCount++;
        Debug.Log(waveCount);
        if (waveCount == maxWaveCount - 1) {
            Debug.Log("Boss");

            // {X¶¬
            StartCoroutine(enemyGenerator.GenerateBoss());

            // {XÄ
            StartCoroutine(ObservateBossState());

        } else {
            // ¶¬ÄJ
            enemyGenerator.SwitchGenerateState(false);

            // ÄÄJ
            StartCoroutine(ObservateGenerateEnemyState());
        }
    }

    /// <summary>
    /// Q[I¹óÔðØèÖ¦
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
