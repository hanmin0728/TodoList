using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static SoundEnum;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Sound Data")]
    [SerializeField] private SoundData soundData;

    [Header("Audio Mixer Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string bgmVolumeParam = "BGMVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    [Header("BGM Settings")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    private Coroutine bgmFadeRoutine;

    [Header("SFX Settings")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private GameObject sfxPrefab;

    private readonly Dictionary<BgmType, AudioClip> bgmCache = new Dictionary<BgmType, AudioClip>();
    private readonly Dictionary<SfxType, AudioClip> sfxCache = new Dictionary<SfxType, AudioClip>();

    private readonly Dictionary<SfxType, float> sfxPlayTimers = new Dictionary<SfxType, float>();
    private const float SfxCooldown = 0.05f;
    
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        if (bgmSource != null && bgmMixerGroup != null)
        {
            bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        }
        InitSoundData();
    }

    private void Start()
    {
        PlayBGM(BgmType.Main, 1f);
        if (SaveManager.HasInstance)
        {
            //firebase 연동 후 데이터처리
            // SetVolume(true, SaveManager.Instance.CurrentData.GetBgmVolume());
            // SetVolume(false, SaveManager.Instance.CurrentData.GetSfxVolume());
        }
    }
    private void InitSoundData()
    {
        if (soundData == null)
        {
            return;
        }

        foreach (var entry in soundData.BgmList)
        {
            if (entry.Type == BgmType.None || entry.Clip == null) continue;
            bgmCache.TryAdd(entry.Type, entry.Clip);
        }

        foreach (var entry in soundData.SfxList)
        {
            if (entry.Type == SfxType.None || entry.Clip == null) continue;
            sfxCache.TryAdd(entry.Type, entry.Clip);
        }
    }

    #region BGM 제어
    public void PlayBGM(BgmType type, float fadeDuration = 1f)
    {
        if (!bgmCache.TryGetValue(type, out AudioClip clip)) return;
        if (bgmSource.clip == clip) return;

        if (bgmFadeRoutine != null)
        {
            StopCoroutine(bgmFadeRoutine);
        }

        bgmFadeRoutine = StartCoroutine(CrossfadeBGM(clip, fadeDuration));
    }
    private IEnumerator CrossfadeBGM(AudioClip nextClip, float duration)
    {
        float startVolume = bgmSource.volume;

        if (bgmSource.clip != null && bgmSource.isPlaying)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
        }

        bgmSource.clip = nextClip;
        bgmSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        bgmSource.volume = 1f;
        bgmFadeRoutine = null;
    }
    #endregion

    #region SFX 제어
    public void PlaySFX(SfxType type, float volume = 1f)
    {
        if (!sfxCache.TryGetValue(type, out AudioClip clip)) return;
        if (sfxPrefab == null) return;

        if (sfxPlayTimers.TryGetValue(type, out float lastPlayTime))
        {
            if (Time.time - lastPlayTime < SfxCooldown) return;
        }
        sfxPlayTimers[type] = Time.time;

        GameObject sfxObj = PoolManager.Instance.Spawn(sfxPrefab, Vector3.zero, Quaternion.identity);

        if (sfxObj.TryGetComponent(out SfxPlayer sfxPlayer))
        {
            sfxPlayer.Play(clip, volume, sfxMixerGroup);
        }
        else
        {
            sfxObj.GetComponent<Poolable>().Release();
        }
    }
    #endregion

    #region 볼륨 제어
    public void SetVolume(bool isBgm, float sliderValue)
    {
        if (audioMixer == null) return;

        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float dbVolume = Mathf.Log10(sliderValue) * 20f;

        string paramName = isBgm ? bgmVolumeParam : sfxVolumeParam;
        audioMixer.SetFloat(paramName, dbVolume);
    }
    #endregion
}
