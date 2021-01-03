using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private EnemyGenerator enemyGenerator;


    private bool isSetUpEnd;

    public bool isGameUp;

    public int waveCount = 0;

    public int maxWaveCount;

    void Start()
    {
        StartCoroutine(ObsevateGenerateEnemyState());
    }

    /// <summary>
    /// �G�̐������Ď�
    /// </summary>
    /// <returns></returns>
    private IEnumerator ObsevateGenerateEnemyState() {
        Debug.Log("�Ď��J�n");
        bool isAllGenerate = false;

        while (!isAllGenerate) {
            if (enemyGenerator.isGenerateEnd) {
                isAllGenerate =true;
            }
            yield return null;
        }
        Debug.Log("�Ď��I��");

        if (waveCount >= maxWaveCount) {
            Debug.Log("Game Clear");
        } else {
            AdvanceWave();
        }   
    }

    /// <summary>
    /// Wave�i�s
    /// </summary>
    private void AdvanceWave() {
        waveCount++;
        Debug.Log(waveCount);
        if (waveCount == maxWaveCount - 1) {
            Debug.Log("Boss");
        } else {
            // �����ĊJ
            enemyGenerator.SwitchGenerateState(false);

            // �Ď��ĊJ
            StartCoroutine(ObsevateGenerateEnemyState());
        }
    }

    /// <summary>
    /// �Q�[���I����Ԃ�؂�ւ�
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
