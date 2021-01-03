using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DefenseBase : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text txtDurability;

    public int durability;

    private int maxDurability;

    [SerializeField]
    private CanvasGroup canvasGroupGameOver;


    void Start()
    {
        canvasGroupGameOver.DOFade(0, 0.1f);
        maxDurability = durability;

        UpdateDisplayDurability();
    }

    /// <summary>
    /// 耐久度の表示更新
    /// </summary>
    private void UpdateDisplayDurability() {
        txtDurability.text = durability + "  / " + maxDurability;

        slider.DOValue((float)durability / maxDurability, 0.25f);

    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Enemy") {
            if (col.gameObject.TryGetComponent(out EnemyController enemyController)) {
                durability -= enemyController.enemyData.power;                
            }

            if (col.gameObject.TryGetComponent(out Bullet bullet)) {
                durability -= bullet.bulletData.bulletPower;
            }
            durability = Mathf.Clamp(durability, 0, maxDurability);
            UpdateDisplayDurability();

            if(durability <= 0) {
                Debug.Log("Game Over");
                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().SwitchGameUp(true);

                DisplayGameOver();
            }
            Destroy(col.gameObject);
        }        
    }

    /// <summary>
    /// ゲームオーバー表示
    /// </summary>
    private void DisplayGameOver() {
        canvasGroupGameOver.DOFade(1.0f, 1.0f);
    }
}

