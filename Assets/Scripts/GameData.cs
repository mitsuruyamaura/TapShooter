using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public int durabilityBase;

    public List<BulletDataSO.BulletData> chooseBulletsList = new List<BulletDataSO.BulletData>();

    [Header("初期設定バレット生成用のデバッグスイッチ")]
    public bool isDebugDefaultBulletCreate;


    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Durabilityの値を取得
    /// </summary>
    /// <returns></returns>
    public int GetDurability() {
        return durabilityBase;
    }

    /// <summary>
    /// バレット選択ポップアップで選択したバレットの情報をセット
    /// </summary>
    /// <param name="bulletDatas"></param>
    public void SetChooseBulletDatas(List<BulletDataSO.BulletData> bulletDatas) {

        for (int i = 0; i < bulletDatas.Count; i++) {
            chooseBulletsList.Add(bulletDatas[i]);
        }
    }
}
