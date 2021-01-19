using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

/// <summary>
/// 音源管理クラス
/// </summary>
public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    // BGM管理
    public enum BGM_Type {
        // BGM用の列挙子をゲームに合わせて登録
        Main,
        Boss,
        Silence = 999
    }

    // SE管理
    public enum SE_Type {
        // SE用の列挙子をゲームに合わせて登録
        BulletDamage_1,
        Treasure_Set,
        Treasure_Get,
        GameClear,
        GameOver,
    }

    public enum StartVoice {
        Start_1,
        Start_2,
    }

    public enum GameUpVoice {
        Loose_1,
        Loose_2,
        Win_1
    }

    public enum AttackVoice {
        Attack_1,
        Attack_2,
        Attack_3
    }

    public enum HitVoice {
        Hit_1,
        Hit_2
    }

    public enum WarningVoice {
        Warning_1,
        Warning_2
    }

    // クロスフェード時間
    public const float CROSS_FADE_TIME = 1.0f;

    // ボリューム関連
    public float BGM_Volume = 0.1f;
    public float SE_Volume = 0.2f;
    public float Voice_Volume = 0.2f;
    public bool Mute = false;

    // === AudioClip ===
    public AudioClip[] BGM_Clips;
    public AudioClip[] SE_Clips;
    public AudioClip[] StartVoice_Clips;
    public AudioClip[] GameUpVoice_Clips;
    public AudioClip[] AttackVoice_Clips;
    public AudioClip[] HitVoice_Clips;
    public AudioClip[] WarningVoice_Clips;


    // SE用AudioMixer  未使用
    public AudioMixer audioMixer;


    // === AudioSource ===
    private AudioSource[] BGM_Sources = new AudioSource[2];
    private AudioSource[] SE_Sources = new AudioSource[16];
    private AudioSource Voice_Source = new AudioSource();

    private bool isCrossFading;

    private int currentBgmIndex = 999;

    void Awake() {
        // シングルトンかつ、シーン遷移しても破棄されないようにする
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // BGM用 AudioSource追加
        BGM_Sources[0] = gameObject.AddComponent<AudioSource>();
        BGM_Sources[1] = gameObject.AddComponent<AudioSource>();

        // SE用 AudioSource追加
        for (int i = 0; i < SE_Sources.Length; i++) {
            SE_Sources[i] = gameObject.AddComponent<AudioSource>();
        }
        Voice_Source = gameObject.AddComponent<AudioSource>();
    }

    void Update() {
        // ボリューム設定
        if (!isCrossFading) {
            BGM_Sources[0].volume = BGM_Volume;
            BGM_Sources[1].volume = BGM_Volume;
        }

        foreach (AudioSource source in SE_Sources) {
            source.volume = SE_Volume;
        }
        Voice_Source.volume = Voice_Volume;
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="bgmType"></param>
    /// <param name="loopFlg"></param>
    public void PlayBGM(BGM_Type bgmType, bool loopFlg = true) {
        // BGMなしの状態にする場合            
        if ((int)bgmType == 999) {
            StopBGM();
            return;
        }

        int index = (int)bgmType;
        currentBgmIndex = index;

        if (index < 0 || BGM_Clips.Length <= index) {
            return;
        }

        // 同じBGMの場合は何もしない
        if (BGM_Sources[0].clip != null && BGM_Sources[0].clip == BGM_Clips[index]) {
            return;
        } else if (BGM_Sources[1].clip != null && BGM_Sources[1].clip == BGM_Clips[index]) {
            return;
        }

        // フェードでBGM開始
        if (BGM_Sources[0].clip == null && BGM_Sources[1].clip == null) {
            BGM_Sources[0].loop = loopFlg;
            BGM_Sources[0].clip = BGM_Clips[index];
            BGM_Sources[0].Play();
        } else {
            // クロスフェード処理
            StartCoroutine(CrossFadeChangeBMG(index, loopFlg));
        }
    }

    /// <summary>
    /// BGMのクロスフェード処理
    /// </summary>
    /// <param name="index">AudioClipの番号</param>
    /// <param name="loopFlg">ループ設定。ループしない場合だけfalse指定</param>
    /// <returns></returns>
    private IEnumerator CrossFadeChangeBMG(int index, bool loopFlg) {
        isCrossFading = true;
        if (BGM_Sources[0].clip != null) {
            // [0]が再生されている場合、[0]の音量を徐々に下げて、[1]を新しい曲として再生
            BGM_Sources[1].volume = 0;
            BGM_Sources[1].clip = BGM_Clips[index];
            BGM_Sources[1].loop = loopFlg;
            BGM_Sources[1].Play();
            BGM_Sources[0].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);

            yield return new WaitForSeconds(CROSS_FADE_TIME);
            BGM_Sources[0].Stop();
            BGM_Sources[0].clip = null;
        } else {
            // [1]が再生されている場合、[1]の音量を徐々に下げて、[0]を新しい曲として再生
            BGM_Sources[0].volume = 0;
            BGM_Sources[0].clip = BGM_Clips[index];
            BGM_Sources[0].loop = loopFlg;
            BGM_Sources[0].Play();
            BGM_Sources[1].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);

            yield return new WaitForSeconds(CROSS_FADE_TIME);
            BGM_Sources[1].Stop();
            BGM_Sources[1].clip = null;
        }
        isCrossFading = false;
    }

    /// <summary>
    /// BGM完全停止
    /// </summary>
    public void StopBGM() {
        BGM_Sources[0].Stop();
        BGM_Sources[1].Stop();
        BGM_Sources[0].clip = null;
        BGM_Sources[1].clip = null;
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SE_Type seType, float volume = 0.2f) {
        int index = (int)seType;
        if (index < 0 || SE_Clips.Length <= index) {
            return;
        }

        // 再生中ではないAudioSouceをつかってSEを鳴らす
        foreach (AudioSource source in SE_Sources) {
            if (source.isPlaying == false) {
                source.clip = SE_Clips[index];
                source.volume = volume;
                source.Play();
                return;
            }
        }
    }

    /// <summary>
    /// SE停止
    /// </summary>
    public void StopSE() {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SE_Sources) {
            source.Stop();
            source.clip = null;
        }
    }

    /// <summary>
    /// BGM一時停止
    /// </summary>
    public void MuteBGM() {
        BGM_Sources[0].Stop();
        BGM_Sources[1].Stop();
    }

    /// <summary>
    /// 一時停止した同じBGMを再生(再開)
    /// </summary>
    public void ResumeBGM() {
        BGM_Sources[0].Play();
        BGM_Sources[1].Play();
    }

    /// <summary>
    /// スタート時のボイス設定
    /// </summary>
    /// <param name="beforeBattleType"></param>
    public void SetStartVoice(StartVoice beforeBattleType) {
        int index = (int)beforeBattleType;
        // ボイス再生
        PlayVoice(StartVoice_Clips, index);
    }

    /// <summary>
    /// ゲーム終了時(クリア、オーバー)のボイス設定
    /// </summary>
    /// <param name="gameUpVoice"></param>
    public void SetGameUpVoice(GameUpVoice gameUpVoice, float volume = 0.2f) {
        int index = (int)gameUpVoice;
        StopVoice();

        // ボイス再生
        PlayVoice(GameUpVoice_Clips, index, volume);
    }

    /// <summary>
    /// 攻撃時のボイス設定
    /// </summary>
    /// <param name="attackVoice"></param>
    public void SetAttackVoice(AttackVoice attackVoice) {
        int index = (int)attackVoice;
        // ボイス再生
        PlayVoice(AttackVoice_Clips, index);
    }

    /// <summary>
    /// 被ダメージ時のヒットボイス設定
    /// </summary>
    /// <param name="hitVoice"></param>
    public void SetHitVoice(HitVoice hitVoice) {
        int index = (int)hitVoice;
        // ボイス再生
        PlayVoice(HitVoice_Clips, index);
    }

    /// <summary>
    /// ボス前のボイス設定
    /// </summary>
    /// <param name="warningVoice"></param>
    public void SetWarningVoice(WarningVoice warningVoice) {
        int index = (int)warningVoice;
        StopVoice();

        // ボイス再生
        PlayVoice(WarningVoice_Clips, index);
    }

    /// <summary>
    /// ボイス再生
    /// </summary>
    /// <param name="playClips"></param>
    /// <param name="index"></param>
    private void PlayVoice(AudioClip[] playClips, int index, float volume = 0.2f) {
        // 再生前の確認
        if (index < 0 || playClips.Length <= index) {
            return;
        }
        if (Voice_Source.isPlaying) {
            return;
        }

        Voice_Source.clip = playClips[index];
        Voice_Source.volume = volume;
        Voice_Source.Play();
    }

    /// <summary>
    /// ボイス停止
    /// </summary>
    public void StopVoice() {
        Voice_Source.Stop();
        Voice_Source.clip = null;
    }


    ////* 未使用 *////

    /// <summary>
    /// AudioMixer設定
    /// </summary>
    /// <param name="vol"></param>
    public void SetAudioMixerVolume(float vol) {
        if (vol == 0) {
            audioMixer.SetFloat("volumeSE", -80);
        } else {
            audioMixer.SetFloat("volumeSE", 0);
        }
    }
}