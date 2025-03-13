using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup Master;
    public AudioMixerGroup Music;
    public AudioMixerGroup Sfx;

    [Header("Audio Sources")]
    public AudioSource bgmSource;   // For background music
    public AudioSource sfxSource;   // For sound effects

    [Header("Audio Clips")]
    public AudioClip defaultBGM;  // Default background music

    public bool randomStart;

    private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.outputAudioMixerGroup = Music;
            bgmSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = Sfx;
            sfxSource.loop = false;
        }

        StartCoroutine(FadeInBGM(4, 1, defaultBGM));
    }

    /// <summary>
    /// Spawn a gameobject and play a sound on it then destroys it
    /// </summary>
    public void PlayOneShotSound(AudioClip clip)
    {
        if (clip == null) return;

        // Create a temporary GameObject to play the sound
        GameObject tempAudio = new GameObject("OneShotAudio");
        AudioSource source = tempAudio.AddComponent<AudioSource>();

        source.clip = clip;
        source.Play();

        // Destroy the GameObject after the clip finishes playing
        Destroy(tempAudio, clip.length);
    }

    /// <summary>
    /// Spawn a gameobject and play a sound on it and add it to a mixer group then destroys it
    /// </summary>
    public void PlayOneShotSound(AudioClip clip, AudioMixerGroup mixerGroup)
    {
        if (clip == null) return;

        // Create a temporary GameObject to play the sound
        GameObject tempAudio = new GameObject("OneShotAudio");
        AudioSource source = tempAudio.AddComponent<AudioSource>();

        source.clip = clip;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        // Destroy the GameObject after the clip finishes playing
        Destroy(tempAudio, clip.length);
    }

    /// <summary>
    /// Plays background music, with optional fade-in.
    /// </summary>
    public void PlayBGM(AudioClip clip, float fadeTime = 1f)
    {
        if (bgmSource.clip == clip) return;

        StartCoroutine(FadeBGM(clip, fadeTime));
    }

    private IEnumerator FadeBGM(AudioClip newClip, float fadeTime)
    {
        // Fade out
        float startVolume = bgmSource.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = 0;
        bgmSource.Stop();

        // Switch music
        bgmSource.clip = newClip;
        bgmSource.Play();

        // Fade in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = startVolume;
    }

    /// <summary>
    /// Stops background music with an optional fade-out.
    /// </summary>
    public void StopBGM(float fadeTime = 1f)
    {
        StartCoroutine(FadeOutBGM(fadeTime));
    }

    private IEnumerator FadeOutBGM(float fadeTime)
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    private IEnumerator FadeInBGM(float fadeTime, float targetVolume, AudioClip newClip)
    {
        // Set music
        bgmSource.clip = newClip;

        // Set random start time
        if (randomStart)
        {
            float randomTime = Random.Range(0f, newClip.length - 10f); // Avoids starting too close to the end
            bgmSource.time = randomTime;
        }

        bgmSource.Play();

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, targetVolume, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = targetVolume;
    }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Adjusts volume for BGM and SFX.
    /// </summary>
    public void SetVolume(float bgmVolume, float sfxVolume)
    {
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Toggles mute for all audio.
    /// </summary>
    public void ToggleMute()
    {
        bool isMuted = bgmSource.mute;
        bgmSource.mute = !isMuted;
        sfxSource.mute = !isMuted;
    }
}
