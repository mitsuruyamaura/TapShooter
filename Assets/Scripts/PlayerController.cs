using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Bullet bulletPrefab;

    public BulletDataSO.BulletData bulletData;

    public int tapCount;

    private GameManager gameManager;

    public BulletDataSO.BulletType currentBulletType;

    [SerializeField]
    private CharaAnimationController charaAnim;


    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        bulletData = DataBaseManager.instance.GetPlayerBulletData(currentBulletType);
    }

    public void SetUpPlayer(GameManager gameManager) {  // <= Start �̑���
        this.gameManager = gameManager;
    }

    void Update() {

        //ShotDebug();


        // �Q�[���N���A�A�Q�[���I�[�o�[�̂����ꂩ�Ȃ�
        if (gameManager.isGameUp) {
            return;
        }

        // �����������ĂȂ��Ȃ�
        if (!gameManager.isSetUpEnd) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            // ��ʂ��^�b�v(�N���b�N)�����ʒu���J�����̃X�N���[�����W�̏���ʂ��ă��[���h���W�ɕϊ�
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // �����̐���(Z�����̏����Ɛ��K��)
            Vector3 direction = Vector3.Scale(tapPos - transform.position, new Vector3(1, 1, 0)).normalized;

            //Debug.Log(direction);

            // �L�����̈ʒu�����������Ɍ���������ꍇ(�L�����̈ʒu�������ɂ����ʂ��^�b�v�����ꍇ)
            if (direction.y <= 0.0f) {
                // �����������Ȃ�
                return;
            }

            // �o���b�g����
            GenerateBullet(direction);

            // �U�����̃{�C�X�Đ�
            SoundManager.instance.SetAttackVoice((SoundManager.AttackVoice)Random.Range(0, SoundManager.instance.AttackVoice_Clips.Length));

            // �U���A�j���Đ�
            charaAnim.PlayAnimation(CharaAnimationController.attackParameter);
        }
    }

    /// <summary>
    /// �e��ύX
    /// </summary>
    /// <param name="newBulletType"></param>
    public void ChangeBullet(BulletDataSO.BulletType newBulletType) {
        currentBulletType = newBulletType;

        bulletData = DataBaseManager.instance.GetPlayerBulletData(currentBulletType);
    }

    /// <summary>
    /// �o���b�g�̐���
    /// </summary>
    /// <param name="direction"></param>
    private void GenerateBullet(Vector3 direction) {
        switch (bulletData.bulletType) {
            case BulletDataSO.BulletType.Player_Normal:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                break;

            case BulletDataSO.BulletType.Player_Blaze:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                break;

            case BulletDataSO.BulletType.Player_3ways_Piercing:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x - 0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;

            case BulletDataSO.BulletType.Player_5ways_Normal:
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x - 0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x - 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, direction);
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x + 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).ShotBullet(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;
        }



        tapCount++;

        if (tapCount >= 50) {
            Debug.Log("�o�[�X�g���");
        }
    }

    private void Generate(Vector3 direction) {
        //Debug.Log("��������ʒu��� : " + direction);

        Bullet bulletObj = Instantiate(bulletPrefab, transform);
        //bulletObj.transform.localPosition = direction;

        //bulletObj.ShotBullet(transform.up);

        bulletObj.ShotBullet(direction);
    }

    private void ShotDebug() {
        if (Input.GetMouseButtonDown(0)) {
            //Instantiate(bulletPrefab, transform).ShotBullet();
            // ��ʂ��^�b�v(�N���b�N)�����ʒu���J�����̃X�N���[�����W�̏���ʂ��ă��[���h���W�ɕϊ�
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Debug.Log("�^�b�v�����ʒu��� : " + tapPos);

            // �������v�Z
            Vector3 direction2 = tapPos - transform.position;

            //Debug.Log("���� : " + direction2);

            // �����̏�񂩂�A�s�v�� Z����(Z�����) �̏������s��
            //direction2 = Vector3.Scale(direction2, new Vector3(1, 1, 0));


            // ���K���������s���A�P�ʃx�N�g���Ƃ���(�����̏��͎����A�����ɂ�鑬�x�����Ȃ����Ĉ��l�ɂ���)
            //direction2 = direction2.normalized;

            //Debug.Log("���K��������̕��� : " + direction2);

            Generate(direction2);
        }
    }
}
