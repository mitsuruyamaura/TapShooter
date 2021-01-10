using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MoveDataSO", menuName = "Create MoveDataSO"),Serializable]
public class MoveDataSO : ScriptableObject
{
    public List<MoveData> moveDataList = new List<MoveData>();

    [Serializable]
    public class MoveData {
        public EnemyDataSO.EnemyType enemyName;
        public float moveSpeed;

        public MoveType moveType;
    }

    [Serializable]
    public enum MoveType {
        A,
        B,
        C
    }

    public UnityAction<Transform> GetMoveEvent(EnemyDataSO.EnemyType enemyType) {
        switch (enemyType) {
            case EnemyDataSO.EnemyType.Normal_0:
                return MoveA;
            case EnemyDataSO.EnemyType.Normal_1:
                return MoveB;
            case EnemyDataSO.EnemyType.Boss:
                return MoveC;
            default:
                return Stop;
        }
    }

    // 移動の種類
    public void MoveA(Transform tran) {
        Debug.Log(tran);
        Debug.Log("移動処理");

        tran.DOLocalMoveY(-3000, 15f);
    }

    public void Stop(Transform tran) {
        Debug.Log(tran);
        Debug.Log("停止");
    }

    public void MoveB(Transform tran) {
        Debug.Log("ジグザグ");
        
        tran.DOLocalMoveX(tran.position.x + 300, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        tran.DOLocalMoveY(-3000, 15f);
    }

    public void MoveC(Transform tran) {
        //float randomValue = UnityEngine.Random.Range(-650, 250);

        //Vector3[] movePosArray = new Vector3[5];
        //movePosArray[0] = new Vector3(tran.localPosition.x, tran.localPosition.y, tran.localPosition.z);
        //movePosArray[1] = new Vector3(tran.localPosition.x, tran.localPosition.y - 1500, tran.localPosition.z);
        //movePosArray[2] = new Vector3(tran.localPosition.x + randomValue, tran.localPosition.y - 1500, tran.localPosition.z);
        //movePosArray[3] = new Vector3(tran.localPosition.x + randomValue, tran.localPosition.y, tran.localPosition.z);
        //movePosArray[4] = new Vector3(tran.localPosition.x, tran.localPosition.y, tran.localPosition.z);

        //tran.DOLocalPath(movePosArray, 10f).SetLoops(-1, LoopType.Yoyo);

        //tran.DOLocalMoveY(-2000, 10f)
        //    .OnComplete(() => {
        //        randomValue = UnityEngine.Random.Range(-250, 250);
        //        tran.DOLocalMoveX(randomValue, 3.0f)
        //        .OnComplete(() => {
        //            tran.DOLocalMoveY(-400, 10f)
        //            .OnComplete(() => {
        //                tran.DOLocalMoveX(-randomValue, 3.0f);
        //            });
        //        });
        //    }).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Unset);

        tran.localPosition = new Vector3(0, tran.localPosition.y, tran.localPosition.z);

        tran.DOLocalMoveY(-500, 3.0f).OnComplete(() => {

            Sequence sequence = DOTween.Sequence();
            sequence.Append(tran.DOLocalMoveX(tran.localPosition.x + 550, 2.5f).SetEase(Ease.Linear));
            sequence.Append(tran.DOLocalMoveX(tran.localPosition.x - 550, 5.0f).SetEase(Ease.Linear));
            sequence.Append(tran.DOLocalMoveX(tran.localPosition.x, 2.5f).SetEase(Ease.Linear));
            sequence.AppendInterval(1.0f).SetLoops(-1, LoopType.Restart);
        });
    }
}
