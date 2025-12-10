using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public AudioMixerGroup mixerGroup; // assign channels
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    public AudioSource SFXSource;
    public AudioSource MusicSource;
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    [Header("Sound Effects")]
    public SoundEffect[] soundEffects;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    
 

    void Awake()
    {
 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


    }

    void Start()
    {
        
    }

   
    public void PlaySound(AudioClip sound)
    {
         if (SFXSource == null)
        {
            Debug.LogError("AudioManager: SFXSource is null!");
            return;
        }

        if (sound == null)
        {
            Debug.LogError("AudioManager: sound is null!");
            return;
        }
        
       
        if (!SFXSource.enabled)
        {
            SFXSource.enabled = true;
        }
        
        SFXSource.clip = sound;
        SFXSource.volume = SFXSource.volume;
        SFXSource.Play();
        
        Debug.Log("AudioManager: Playing sound - " + sound.name);
    }

    
    // Volume controls
    public void SetMusicVolume(float volume)
    {
        if (MusicSource != null)
            MusicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (SFXSource != null)
            SFXSource.volume = Mathf.Clamp01(volume);
    }

    // music controls
    public void StopMusic()
    {
        if (MusicSource != null)
            MusicSource.Stop();
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (MusicSource == null)
        {
            Debug.LogError("AudioManager: musicSource is null!");
            return;
        }

        if (musicClip == null)
        {
            Debug.LogError("AudioManager: musicClip is null!");
            return;
        }

        MusicSource.Stop();
        MusicSource.clip = musicClip;
        MusicSource.loop = loop;
        MusicSource.volume = musicVolume;
        MusicSource.Play();
        
        Debug.Log("AudioManager: Playing music - " + musicClip.name);
    }

    public void PauseMusic()
    {
        if (MusicSource != null)
            MusicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (MusicSource != null)
            MusicSource.UnPause();
    }

    public void FadeOutMusic(float duration)
    {
        if (MusicSource != null)
            StartCoroutine(FadeOutCoroutine(duration));
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = MusicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            MusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        MusicSource.Stop();
        MusicSource.volume = startVolume; // reset for next play
    }

    internal static void PlayDelayedSound(AudioClip audioClip, float delay)
    {
        if (Instance == null)
        {
            Debug.LogError("AudioManager instance is null!");
            return;
        }

        Instance.StartCoroutine(Instance.PlayDelayedSoundCoroutine(audioClip, delay));
    }

    private System.Collections.IEnumerator PlayDelayedSoundCoroutine(AudioClip audioClip, float delay)
    {
        yield return new WaitForSeconds(delay);
        Instance.PlaySound(audioClip);
    }
}
