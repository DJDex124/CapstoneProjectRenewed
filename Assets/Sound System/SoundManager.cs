using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager current;

    [Header("SFX")]
    public sound[] sfxLibrary;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Music")]
    public sound[] musicLibrary;
    public AudioSource musicSource;
    [Range(0f, 1f)] public float musicVolume = 1f;

    [Header("Audio Mixers")]
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup musicMixer;

    private sound currentMusic;
    private Dictionary<string, sound> sfxDict = new Dictionary<string, sound>();
    private Dictionary<string, sound> musicDict = new Dictionary<string, sound>();

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

       
        foreach (sound s in sfxLibrary)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.maxVolume * sfxVolume;
            s.source.playOnAwake = false;
            s.source.outputAudioMixerGroup = sfxMixer;
            sfxDict[s.name] = s;
        }

       
        foreach (sound s in musicLibrary)
        {
            musicDict[s.name] = s;
        }

        
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicMixer;
        }

        PlayMusic("MenuMusic", true);
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

    
    public void PlaySFX(string name)
    {
        if (!sfxDict.TryGetValue(name, out sound s))
        {
            Debug.LogWarning($"SFX '{name}' not found.");
            return;
        }

        s.source.volume = s.maxVolume * sfxVolume;

        
        if (s.stoppable)
            s.source.Play();
        else
            s.source.PlayOneShot(s.clip);
    }

    
    public void PlaySFXAt(string name, Vector3 position)
    {
        if (!sfxDict.TryGetValue(name, out sound s))
        {
            Debug.LogWarning($"SFX '{name}' not found.");
            return;
        }

        GameObject tempGO = new GameObject($"TempAudio_{name}");
        tempGO.transform.position = position;

        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.outputAudioMixerGroup = sfxMixer;
        tempSource.spatialBlend = 1f; // full 3D
        tempSource.volume = s.maxVolume * sfxVolume;
        tempSource.pitch = GetRandomPitch(s);
        tempSource.clip = s.clip;
        tempSource.Play();

        Destroy(tempGO, s.clip.length);
    }

  
    public void StopSFX(string name)
    {
        if (!sfxDict.TryGetValue(name, out sound s))
        {
            Debug.LogWarning($"SFX '{name}' not found.");
            return;
        }
        s.source.Stop();
    }

  
    public bool IsSFXPlaying(string name)
    {
        if (!sfxDict.TryGetValue(name, out sound s))
        {
            Debug.LogWarning($"SFX '{name}' not found.");
            return false;
        }
        return s.source.isPlaying;
    }

    
    public void PlayMusic(string name, bool fadeIn = false)
    {
        if (!musicDict.TryGetValue(name, out sound s))
        {
            Debug.LogWarning($"Music '{name}' not found.");
            return;
        }

        musicSource.clip = s.clip;
        musicSource.loop = s.loop;
        currentMusic = s;

        if (fadeIn)
        {
            StartCoroutine(StartFade(musicSource, 0f, s.maxVolume * musicVolume, 3f));
        }
        else
        {
            musicSource.volume = s.maxVolume * musicVolume;
            musicSource.Play();
        }
    }

    
    public void CrossfadeToMusic(string name, float duration = 1.5f)
    {
        if (!musicDict.TryGetValue(name, out sound next))
        {
            Debug.LogWarning($"Music '{name}' not found.");
            return;
        }
        StartCoroutine(CrossfadeCoroutine(next, duration));
    }

    private IEnumerator CrossfadeCoroutine(sound next, float duration)
    {
        // Fade out current track
        yield return StartCoroutine(StartFade(musicSource, musicSource.volume, 0f, duration));

        // Swap clip
        musicSource.clip = next.clip;
        musicSource.loop = next.loop;
        currentMusic = next;

        // Fade in next track
        yield return StartCoroutine(StartFade(musicSource, 0f, next.maxVolume * musicVolume, duration));
    }

  
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxMixer.audioMixer.SetFloat("sfxVolume", sfxVolume);
        foreach (sound s in sfxLibrary)
            s.source.volume = sfxVolume * s.maxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicMixer.audioMixer.SetFloat("musicVolume", musicVolume);
        if (currentMusic != null)
            musicSource.volume = musicVolume * currentMusic.maxVolume;
    }

   
    public static IEnumerator StartFade(AudioSource source, float startVolume, float targetVolume, float duration)
    {
        source.volume = startVolume;

        if (targetVolume > 0f && !source.isPlaying)
            source.Play();

        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        source.volume = targetVolume;

        if (targetVolume <= 0f)
            source.Stop();
    }

    private float GetRandomPitch(sound s)
    {
        return 1f + Random.Range(-s.pitchVariance, s.pitchVariance);
    }
}


[System.Serializable]
public class sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)] public float maxVolume = 1f;
    [Range(0f, 0.5f)] public float pitchVariance = 0f; 
    public bool loop = false;
    public bool stoppable = false; // if true, uses Play() instead of PlayOneShot so StopSFX works

    [HideInInspector] public AudioSource source;
}