using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public MoveDataSO moveDataSO;

    public EnemyDataSO enemyDataSO;

    public BulletDataSO bulletDataSO;

    private Transform temporaryObjectContainerTran;

    private Transform treasureBoxIconTran;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        Initialize();
    }

    /// <summary>
    /// ����������
    /// </summary>
    public void Initialize() {
        // TODO ��������Βǉ�

        // �V�[���̊J�n�Ɏ擾(�V���O���g���N���X�ł��邽�߁A�ăX�^�[�g�����ꍇ�ɂ��擾����)
        temporaryObjectContainerTran = GameObject.FindGameObjectWithTag("TemporaryObjectContainer").transform;

        treasureBoxIconTran = GameObject.FindGameObjectWithTag("TreasureBox").transform;
    }

    /// <summary>
    /// EnemyData�擾�p
    /// </summary>
    /// <param name="enemyType"></param>
    /// <returns></returns>
    public EnemyDataSO.EnemyData GetEnemyData(EnemyDataSO.EnemyType enemyType) {
        Debug.Log(enemyType);
        foreach (EnemyDataSO.EnemyData enemyData in enemyDataSO.enemyDataList.Where(x => x.enemyType == enemyType)) {
            return enemyData;
        }
        return null;
    }

    /// <summary>
    /// EnemyData����BulletData�擾�p
    /// </summary>
    /// <param name="enemyData"></param>
    /// <returns></returns>
    public BulletDataSO.BulletData GetEnemyBulletData(EnemyDataSO.EnemyData enemyData) {
        foreach (BulletDataSO.BulletData bulletData in bulletDataSO.bulletDataList.Where((x) => x.bulletType == enemyData.bulletType)) {
            return bulletData;
        }
        return null;
    }

    /// <summary>
    /// BulletType����BulletData�擾�p
    /// </summary>
    /// <param name="bulletType"></param>
    /// <returns></returns>
    public BulletDataSO.BulletData GetPlayerBulletData(BulletDataSO.BulletType bulletType) {
        foreach (BulletDataSO.BulletData bulletData in bulletDataSO.bulletDataList.Where((x) => x.bulletType == bulletType)) {
            return bulletData;
        }
        return bulletDataSO.bulletDataList[0];
    }

    /// <summary>
    /// ElementType����Sprite�擾�p
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    public Sprite GetElementTypeSprite(ElementType elementType) {
        foreach(BulletDataSO.Element element in bulletDataSO.elementList.Where(x => x.no == (int)elementType)) {
            return element.elementSprite;
        }
        return bulletDataSO.elementList[0].elementSprite;
    }

    /// <summary>
    /// �󔠃A�C�R���̈ʒu���̎擾
    /// </summary>
    /// <returns></returns>
    public Transform GetTresureBoxIconTransfrom() {
        return treasureBoxIconTran;
    }

    /// <summary>
    /// �ꎞ�I�u�W�F�N�g(�o���b�g�A�G�t�F�N�g�A�󔠂Ȃ�)�̊i�[�p�Q�[���I�u�W�F�N�g�̈ʒu���̎擾
    /// </summary>
    /// <returns></returns>
    public Transform GetTemporaryObjectContainerTransform() {
        return temporaryObjectContainerTran;
    }
}
