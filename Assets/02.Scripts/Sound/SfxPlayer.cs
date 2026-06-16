using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SfxPlayer : Poolable
{
    private AudioSource audioSource;
    private Coroutine releaseRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void Play(AudioClip clip, float volume, AudioMixerGroup mixerGroup)
    {
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
        }
        releaseRoutine = StartCoroutine(ReleaseAfterDelay(clip.length));
    }

    private IEnumerator ReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Release();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
}
