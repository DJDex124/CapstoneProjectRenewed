using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager current;

    [Header("SFX")]
    public Sound[] sfxLibrary;
    
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Music")]
    public Sound[] musicLibrary;
    public AudioSource musicSource;
    [Range(0f, 1f)] public float musicVolume = 1f;

    [Header("Audio Mixers")]
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup musicMixer;

    private Sound currentMusic;
    private Dictionary<string, Sound> sfxDict = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> musicDict = new Dictionary<string, Sound>();

    private void Awake()
    {
       
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
       

        foreach (Sound S in sfxLibrary)
        {
            sfxDict[S.name] = S;
        }

        foreach (Sound S in musicLibrary)
        {
            musicDict[S.name] = S;
        }

        
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicMixer;
        }

        
    }
    public void PlayLoop(string name, AudioSource target)
    {
        if (!sfxDict.TryGetValue(name, out Sound S))
        {
            Debug.LogWarning($"SFX '{name}' not found.");
            return;
        }
        if (target == null) return;

        // Already playing this exact clip? Do nothing.
        if (target.isPlaying && target.clip == S.clip)
            return;

        target.outputAudioMixerGroup = sfxMixer;
        target.spatialBlend = 1f;
        target.volume = S.maxVolume * sfxVolume;
        target.pitch = GetRandomPitch(S);
        target.clip = S.clip;
        target.loop = true;
        target.Play();
    }
    public void PlayOneShotSFX(string name, AudioSource target)
    {
        if (!sfxDict.TryGetValue(name, out Sound S))
        {
            Debug.LogWarning($"SFX '{name}' not found.");
            return;
        }
        if (target == null) return;

        target.outputAudioMixerGroup = sfxMixer;
        target.spatialBlend = 1f;
        target.pitch = GetRandomPitch(S);
        target.PlayOneShot(S.clip, S.maxVolume * sfxVolume);
    }
    public void StopLoop(AudioSource target)
    {
        if (target != null && target.isPlaying)
            target.Stop();
    }
    public void selectMusic(string name, AudioSource target)
    {
        if (!musicDict.TryGetValue(name, out Sound S))
        {
            Debug.LogWarning($"Music '{name}' not found.");
            return;
        }
        if (target == null) return;

        // Already playing this exact clip? Do nothing.
        if (target.isPlaying && target.clip == S.clip)
            return;

        target.outputAudioMixerGroup = sfxMixer;
        target.volume = S.maxVolume * sfxVolume;
        target.clip = S.clip; 
        
    }
    
    public IEnumerator fadeMusicOut(AudioSource target)
    {
        if (target == null) yield break;
        float startVolume = target.volume;
        while (target.volume > 0)
        {
            target.volume -= startVolume * Time.deltaTime / 2; // Fade out over 2 seconds
            yield return null;
        }
        target.Stop();
        
    }
    public IEnumerator fadeMusicIn(AudioSource target, float targetVolume)
    {
        if (target == null) yield break;
        target.volume = 0;
        target.Play();
        while (target.volume < targetVolume)
        {
            target.volume += targetVolume * Time.deltaTime / 2; // Fade in over 2 seconds
            yield return null;
        }
    }

    public void pauseMusic(AudioSource target)
    {
        if (target != null && target.isPlaying)
            target.Pause();
    }
    public void resumeMusic(AudioSource target)
    {
        if (target != null && !target.isPlaying)
            target.UnPause();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicMixer;
        }
    }

    

  

   
  
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxMixer.audioMixer.SetFloat("sfxVolume", sfxVolume);
        foreach (Sound s in sfxLibrary)
            s.source.volume = sfxVolume * s.maxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicMixer.audioMixer.SetFloat("musicVolume", musicVolume);
        if (currentMusic != null)
            musicSource.volume = musicVolume * currentMusic.maxVolume;
    }

    private float GetRandomPitch(Sound s)
    {
        return 1f + Random.Range(-s.pitchVariance, s.pitchVariance);
    }
}


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)] 
    public float maxVolume = 1f;
    [Range(0f, 0.5f)] 
    public float pitchVariance = 0f; 
    public bool loop = false;
    public bool stoppable = false; // if true, uses Play() instead of PlayOneShot so StopSFX works

    [HideInInspector] public AudioSource source;
}