using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Bullet bulletPrefab;

    public BulletDataSO.BulletData bulletData;

    public int tapCount;

    private GameManager gameManager;

    public BulletDataSO.BulletType currentBulletType;



    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        bulletData = DataBaseManager.instance.GetPlayerBulletData(currentBulletType);
    }


    void Update() {
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

    private void GenerateBullet(Vector3 direction) {
        switch (bulletData.bulletType) {
            case BulletDataSO.BulletType.D:
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                break;
            case BulletDataSO.BulletType.E:
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                break;
            case BulletDataSO.BulletType.F:
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x -0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;
            case BulletDataSO.BulletType.G:
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x - 0.5f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x - 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, direction);
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x + 0.25f, direction.y, direction.z));
                Instantiate(bulletPrefab, transform).Shot(bulletData, new Vector3(direction.x + 0.5f, direction.y, direction.z));
                break;
        }



        tapCount++;

        if (tapCount >= 50) {
            Debug.Log("�o�[�X�g���");
        }
    }
}
