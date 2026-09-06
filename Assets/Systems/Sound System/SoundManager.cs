using System.Collections;
using System.Collections.Generic;
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