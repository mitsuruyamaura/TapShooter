using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    public BulletDataSO.BulletData bulletData;

    [SerializeField]
    private Image imgBullet;

    private bool isTarget;

    private Vector3 nearPos;

    /// <summary>
    /// �o���b�g�̐���
    /// </summary>
    /// <param name="bulletData"></param>
    /// <param name="direction"></param>
    public void Shot(BulletDataSO.BulletData bulletData, Vector3 direction) {

        // �ǔ��e�̂ݏ�������
        if (bulletData.bulletType == BulletDataSO.BulletType.Player_Blaze) {

            // ���ˎ��ɃQ�[�����ɂ���G�l�~�[�̏����擾
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // �G�l�~�[���P�̈ȏア��Ȃ�
            if (enemies.Length > 0) {

                // ��ԋ߂��G�l�~�[�Ƃ��āA�z��̍ŏ��̃G�l�~�[�̈ʒu�������ɓo�^
                nearPos = enemies[0].transform.position;

                // ���ׂẴG�l�~�[�̈ʒu�����ԂɊm�F
                for (int i = 0; i < enemies.Length; i++) {
                    // ����͏ȗ��\
                    Vector3 pos = enemies[i].transform.position;
                    // ���݂̃G�l�~�[�̈ʒu�Ɣ�r���āA��ʂ̉��ɋ߂����̂� nearPos �Ƃ��čX�V����
                    if (nearPos.x < pos.x && nearPos.y < pos.y) {
                        nearPos = pos;
                        //Debug.Log(nearPos);
                    }
                }

                // �ǔ�����^�[�Q�b�g����Ƃ��ēo�^���AUpdate���\�b�h�𓮂��悤�ɂ���
                isTarget = true;
            }

        }

        this.bulletData = bulletData;
        imgBullet.sprite = bulletData.bulletSprite;

        if (bulletData.liberalType == BulletDataSO.LiberalType.Player) {
            //transform.eulerAngles = new Vector3(0, 0, 180);
            transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        }

        if (bulletData.liberalType == BulletDataSO.LiberalType.BOSS) {
            //transform.SetParent(GameObject.FindGameObjectWithTag("BulletPool").transform);
            transform.SetParent(DataBaseManager.instance.GetTemporaryObjectContainerTransform());
        }

        // �ǔ��e�ȊO
        if (bulletData.bulletType != BulletDataSO.BulletType.Player_Blaze) {
            GetComponent<Rigidbody2D>().AddForce(direction * bulletData.bulletSpeed);
        } 
        //else {
            //dir = Vector3.Scale(nearPos - transform.position, new Vector3(1, 1, 0)).normalized;
            //rad = Mathf.Atan2(dir.y, dir.x);

            //Debug.Log(rad);
        //}

        Destroy(gameObject, 5.0f);
    }

    private void Update() {
        if (!isTarget) {
            return;
        }

        // �ǔ��p(�^�b�v�ʒu�Ɋ֌W�Ȃ��A�L�����ɋ߂��G�l�~�[�ɔ��˂���)
        Vector3 currentPos = transform.position;
        Vector3 targetPos = nearPos;

        transform.position = Vector3.MoveTowards(currentPos, targetPos, Time.deltaTime / 10 * bulletData.bulletSpeed);

        //// ���݈ʒu��Position�ɑ��
        //Position = transform.localPosition;
        //// x += SPEED * cos(���W�A��)
        //// y += SPEED * sin(���W�A��)
        //// ����œ���̕����֌������Đi��ł����B
        //Position.x += bulletData.bulletSpeed * Mathf.Cos(rad);
        //Position.y += bulletData.bulletSpeed * Mathf.Sin(rad);
        //// ���݂̈ʒu�ɉ��Z���Z���s����Position��������
        //transform.localPosition = Position;
    }
}
