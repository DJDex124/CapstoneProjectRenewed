using JetBrains.Annotations;
using System.Collections;
using UnityEngine;


public enum playerAudio
{
    WALK,
    JUMP,
    LAND,
    SWING,
    HIT,
    DEATH,

}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioClip[] soundList;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySound(playerAudio sound, float volume = 1)
    {
       instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
    
}
public class Audio
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 1f)]
    public float pitch;


    public bool loop;    
    public bool PlayOnAwake;
    public bool IsPlaying;

    public AudioSource AudioSource;
}
