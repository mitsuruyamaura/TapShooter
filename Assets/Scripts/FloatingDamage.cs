using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingDamage : MonoBehaviour
{
    [SerializeField]
    private Text txtFloatingDamage;

    /// <summary>
    /// ダメージ表示の制御
    /// </summary>
    /// <param name="damage"></param>
    public void DisplayFloatingDamage(int damage) {
        transform.localPosition = new Vector3(transform.localPosition.x + Random.Range(-20, 20), transform.localPosition.y + Random.Range(-10, 10), 0);

        txtFloatingDamage.text = damage.ToString();

        transform.DOLocalMoveY(transform.localPosition.y + 50, 1.0f).OnComplete(() => { Destroy(gameObject); });
    }
}
