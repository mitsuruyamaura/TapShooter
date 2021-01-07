using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private EnemyGenerator enemyGenerator;

    [SerializeField]
    private BulletSelectManager bulletSelectManager;

    [SerializeField]
    private PlayerController playerController;

    public bool isSetUpEnd;

    public bool isGameUp;

    public int waveCount = 0;

    public int maxWaveCount;

    [SerializeField]
    private CanvasGroup canvasGroupGameClear;

    [SerializeField]
    private Image imgGameClear;

    IEnumerator Start()
    {
        isSetUpEnd = false;

        // �Q�[���N���A�Z�b�g�̏����ݒ�
        canvasGroupGameClear.alpha = 0;
        imgGameClear.transform.localScale = Vector3.zero;

        // EnemyGenerator�̏����ݒ�
        enemyGenerator.SetUpEnemyGenerator(playerController, this);

        // �o���b�g�̃{�^���𐶐�
        yield return StartCoroutine(bulletSelectManager.GenerateBulletSelectDetail(playerController));

        // �g�p�ł���o���b�g�̊m�F�ƍX�V
        bulletSelectManager.JugdeOpenBullets();

        isSetUpEnd = true;

        StartCoroutine(ObservateGenerateEnemyState());
    }

    /// <summary>
    /// �G�̐������Ď�
    /// </summary>
    /// <returns></returns>
    private IEnumerator ObservateGenerateEnemyState() {
        Debug.Log("�Ď��J�n");
        bool isAllGenerate = false;

        while (!isAllGenerate) {
            if (enemyGenerator.isGenerateEnd) {
                isAllGenerate = true;
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

    private IEnumerator ObservateBossState() {
        Debug.Log("�{�X�Ď��J�n");

        while (!enemyGenerator.isBossDestroyed) {
            yield return null;
        }

        Debug.Log("�Q�[���N���A");

        // �Q�[���N���A����X�V
        SwitchGameUp(true);

        // �N���A�\��
        DisplayGameClear();

        // ���ׂẴo���b�g�������Ȃ���Ԃɂ���
        bulletSelectManager.InactivateAllBulletBtns();
    }

    /// <summary>
    /// Wave�i�s
    /// </summary>
    private void AdvanceWave() {
        waveCount++;
        Debug.Log(waveCount);
        if (waveCount == maxWaveCount - 1) {
            Debug.Log("Boss");

            // �{�X����
            StartCoroutine(enemyGenerator.GenerateBoss());

            // �{�X�Ď�
            StartCoroutine(ObservateBossState());

        } else {
            // �����ĊJ
            enemyGenerator.SwitchGenerateState(false);

            // �Ď��ĊJ
            StartCoroutine(ObservateGenerateEnemyState());
        }
    }

    /// <summary>
    /// �Q�[���I����Ԃ�؂�ւ�
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchGameUp(bool isSwitch) {
        isGameUp = isSwitch;

        if (isGameUp) {
            // �G�l�~�[�����ׂč폜
            enemyGenerator.ClearEnemyList();
        }
    }

    /// <summary>
    /// �Q�[���N���A�\��
    /// </summary>
    private void DisplayGameClear() {
        canvasGroupGameClear.DOFade(1.0f, 0.25f)
            .OnComplete(()=> 
            {
                imgGameClear.transform.DOPunchScale(Vector3.one * 2.5f, 0.5f)
                    .OnComplete(()=> 
                    {
                        imgGameClear.transform.DOShakeScale(0.5f);
                        imgGameClear.transform.localScale = Vector3.one * 1.5f;
                    });
            });
    }

    void Update()
    {
        
    }
}
