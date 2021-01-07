using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingMessage : MonoBehaviour
{
    [SerializeField]
    private Text txtFloatingMessage;

    public enum FloatingMessageType {
        EnemyDamage,
        PlayerDamage,
        GetExp,
        BulletCost,
    }

    /// <summary>
    /// ダメージ表示の制御
    /// </summary>
    /// <param name="damage"></param>
    public void DisplayFloatingDamage(int damage, FloatingMessageType floatingMessageType = FloatingMessageType.EnemyDamage) {
        transform.localPosition = new Vector3(transform.localPosition.x + Random.Range(-20, 20), transform.localPosition.y + Random.Range(-10, 10), 0);

        txtFloatingMessage.text = damage.ToString();

        txtFloatingMessage.color = GetMessageColor(floatingMessageType);

        transform.DOLocalMoveY(transform.localPosition.y + 50, 1.0f).OnComplete(() => { Destroy(gameObject); });
    }

    private Color GetMessageColor(FloatingMessageType floatingMessageType) {
        switch (floatingMessageType) {
            case FloatingMessageType.EnemyDamage:
            case FloatingMessageType.PlayerDamage:
                return Color.red;
            case FloatingMessageType.GetExp:
                return Color.yellow;
            case FloatingMessageType.BulletCost:
                return Color.blue;
        }
        return Color.red;
    }
}
