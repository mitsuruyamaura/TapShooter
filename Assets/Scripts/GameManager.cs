using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private CanvasGroup canvasGroupOpeningFilter;

    [SerializeField]
    private Image imgGameStart;

    [SerializeField]
    private GameObject fireworksPrefab;

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private Button btnRestart;

    private bool isClickable;

    [SerializeField]
    private CanvasGroup canvasGroupRestartImage;

    [SerializeField]
    private Text txtTresureBoxCount;

    private int tresureBoxCount;

    [SerializeField]
    private GameObject charaObj;


    IEnumerator Start()
    {
        // �����J�n
        isSetUpEnd = false;

        // �L�����̌��݂̃T�C�Y��ێ����A�ꎞ�A�����Ȃ�����
        float scaleX = charaObj.transform.localScale.x;
        charaObj.transform.localScale = Vector3.zero;

        // �Q�[���N���A�Z�b�g�̏����ݒ�
        canvasGroupGameClear.alpha = 0;
        imgGameClear.transform.localScale = Vector3.zero;
        btnRestart.onClick.AddListener(() => OnClickRestart(canvasGroupGameClear));
        canvasGroupRestartImage.alpha = 0;

        // EnemyGenerator�̏����ݒ�
        enemyGenerator.SetUpEnemyGenerator(playerController, this);

        // �o���b�g�̃{�^���𐶐�
        yield return StartCoroutine(bulletSelectManager.GenerateBulletSelectDetail(playerController));

        // �g�p�ł���o���b�g�̊m�F�ƍX�V
        //bulletSelectManager.JugdeOpenBullets();

        // �Q�[���X�^�[�g���̉��o
        yield return StartCoroutine(Opening());

        // �L�����\��(�X�^�[�g���o�̓r���ŃL�������^�C�~���O�悭�\��������)
        charaObj.transform.DOScale(Vector3.one * scaleX, 1.0f).SetEase(Ease.Linear);
        
        // �X�^�[�g���o���I������܂ňꎞ�����𒆒f���đҋ@(���̊ԂɃL�����͌��̑傫���ɖ߂��Ă���)
        yield return new WaitForSeconds(1.5f);

        // �g�p�ł���o���b�g�̊m�F�ƍX�V�B�����o���b�g���A�j��������
        bulletSelectManager.JugdeOpenBullets();

        // ����������Ԃɂ��āA��ʂ̃^�b�v���󂯕t����
        isSetUpEnd = true;

        // �G�l�~�[�̐����̊Ď��X�^�[�g
        StartCoroutine(ObservateGenerateEnemyState());
    }

    private IEnumerator Opening() {
        canvasGroupOpeningFilter.DOFade(0.0f, 1.0f)
            .OnComplete(() => 
            {
                imgGameStart.transform.DOLocalJump(Vector3.zero, 300.0f, 3, 1.5f).SetEase(Ease.Linear);
                //imgGameStart.transform.DOLocalMoveX(0, 1.0f);
            });

        yield return new WaitForSeconds(3.5f);
        imgGameStart.transform.DOLocalJump(new Vector3(1500, 0, 0), 200.0f, 3, 1.5f).SetEase(Ease.Linear);
        //imgGameStart.transform.DOLocalMoveX(1500, 1.0f);
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

        // �N���A���o
        StartCoroutine(GenerateFireWorks());
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

            // �G�̃o���b�g�����ׂč폜
            enemyGenerator.DestroyTemporaryObjectContainer();
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

                        // ��ʃ^�b�v������
                        canvasGroupGameClear.blocksRaycasts = true;

                        // Restart�̓_�ŕ\��
                        canvasGroupRestartImage.DOFade(1.0f, 1.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                    });
            });
    }

    void Update()
    {
        
    }

    /// <summary>
    /// �ԉ΂̐���
    /// </summary>
    private IEnumerator GenerateFireWorks() {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < Random.Range(5, 8); i++) {
            GameObject fireworks = Instantiate(fireworksPrefab, canvasTran, false);
            
            // �F��ύX         
            ParticleSystem.MainModule main = fireworks.GetComponent<ParticleSystem>().main;
            main.startColor = new ParticleSystem.MinMaxGradient(GetNewParticleColor());

            // �ʒu�ύX
            fireworks.transform.localPosition = new Vector3(fireworks.transform.localPosition.x + Random.Range(-500, 500), fireworks.transform.localPosition.y + Random.Range(700, 1000));
            
            Destroy(fireworks, 3f);
            
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// �p�[�e�B�N���̐F�������_���Őݒ�
    /// </summary>
    /// <returns></returns>
    private Color GetNewParticleColor() {
        int r = Random.Range(0, 255);
        int g = Random.Range(0, 255);
        int b = Random.Range(0, 255);
        Color newColor = new Color32((byte)r, (byte)g, (byte)b, 255);
        return newColor;         
    }

    /// <summary>
    /// �Q�[���N���A��̃^�b�v����
    /// </summary>
    public void OnClickRestart(CanvasGroup canvasGroup) {
        // �N���b�N�ςȂ珈�����Ȃ�
        if (isClickable) {
            return;
        }

        // �d���h�~
        isClickable = true;

        // ���X�^�[�g
        StartCoroutine(RestartGame(canvasGroup));
    }

    /// <summary>
    /// ���X�^�[�g
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestartGame(CanvasGroup canvasGroup) {

        canvasGroup.DOFade(0, 1.5f);
        yield return new WaitForSeconds(1.5f);

        // �C�x���g�ɃC�x���g�n���h���[��ǉ�
        SceneManager.sceneLoaded += SceneLoaded;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// �C�x���g�n���h���[�B�V�[���J�ڎ��Ɏ����I�Ɏ��s�����
    /// </summary>
    /// <param name="nextScene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene nextScene, LoadSceneMode mode) {
        DataBaseManager.instance.Initialize();
    }

    /// <summary>
    /// �󔠂̊l���������Z���ĕ\���X�V
    /// </summary>
    public void UpdateTreasureBoxCount() {
        tresureBoxCount++;
        Debug.Log("���݂̊l���󔠐� : " + tresureBoxCount);

        txtTresureBoxCount.text = tresureBoxCount.ToString();
    }
}