using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class EnemyController : MonoBehaviour
{
    [Header("エネミーのHP")]
    public int hp;

    void Update() {
        transform.Translate(0, -0.01f, 0);
        if (transform.localPosition.y < -1500) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        // バレットが接触したら
        if (col.gameObject.tag == "Bullet") {

            //Debug.Log("侵入したオブジェクト名 : " + col.gameObject.name);
            //Debug.Log("侵入したオブジェクトのタグ名 : " + col.gameObject.tag);

            // バレットの破壊処理を呼び出す(メソッド名を修正すれば、一緒に修正される)
            DestroyBullet(col);   //　<=　☆①　後程、メソッド名を右クリックのメニューから変更すると修正されるので、最初は前のままでよいが、後で確認する


            ////*  ここから追加  *////


            // 侵入してきたコライダーのゲームオブジェクトに Bullet スクリプトがアタッチされていたら取得して bullet 変数に代入して、if 文の中の処理を行う
            if (col.gameObject.TryGetComponent(out Bullet bullet)) {

                // HPの更新処理とエネミーの破壊確認の処理を呼び出す
                UpdateHp(bullet);           //  <=  ☆①　UpdateHp メソッドに送る引数の情報を追加　
            }
        }
    }

    /// <summary>
    /// バレットの破壊処理
    /// </summary>
    private void DestroyBullet(Collider2D col)　　　//　<=　☆①　メソッド名を右クリックのメニューから変更する。呼び出し元の名前も変更になっているか確認する
    {
        // 侵入判定の確認
        //Debug.Log("侵入したオブジェクト名 : " + col.gameObject.tag);

        // バレットを破壊する
        Destroy(col.gameObject);
    }

    /// <summary>
    /// Hpの更新処理とエネミーの破壊確認処理
    /// </summary>
    private void UpdateHp(Bullet bullet) {
        // hpの減算処理
        hp -= 15;

        if (hp <= 0) {
            hp = 0;

            // このゲームオブジェクトを破壊する
            Destroy(gameObject);
        } else {

            Debug.Log("残り Hp : " + hp);
        }
    }
}
