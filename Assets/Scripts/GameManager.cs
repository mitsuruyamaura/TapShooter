using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private EnemyGenerator enemyGenerator;


    private bool isSetUpEnd;

    public int waveCount = 0;

    public int maxWaveCount;

    void Start()
    {
        StartCoroutine(ObsevateGenerateEnemyState());
    }

    /// <summary>
    /// GÌ¶¬ðÄ
    /// </summary>
    /// <returns></returns>
    private IEnumerator ObsevateGenerateEnemyState() {
        Debug.Log("ÄJn");
        bool isAllGenerate = false;

        while (!isAllGenerate) {
            if (enemyGenerator.isGenerateEnd) {
                isAllGenerate =true;
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

    /// <summary>
    /// Waveis
    /// </summary>
    private void AdvanceWave() {
        waveCount++;
        Debug.Log(waveCount);
        if (waveCount == maxWaveCount - 1) {
            Debug.Log("Boss");
        } else {
            // ¶¬ÄJ
            enemyGenerator.SwitchGenerateState(false);

            // ÄÄJ
            StartCoroutine(ObsevateGenerateEnemyState());
        }
    }

    void Update()
    {
        
    }
}
