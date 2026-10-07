using System.Collections;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private SoundManager SM;
    public AudioSource audioSource;
    [SerializeField] private string[] soundList;
    [SerializeField] private string singleSound;
    private string randomSound;
    
    [SerializeField] private SoundType soundType;
    
    void Start()
    {
        SM = SoundManager.current;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        if (soundType == SoundType.Loop)
        {
            SM.PlayLoop(singleSound, audioSource);
        }
        if (soundType == SoundType.Random)
        {
            playRandomSound();
        }
        if (soundType == SoundType.Music)
        {
            StartCoroutine(musicPattern()); 
        }
    }
    private void Update()
    {
        if (Time.deltaTime <= 0)
        {
            audioSource.Pause();
        }
        else if (Time.deltaTime > 0)
        {
            if (audioSource.isPlaying)
            {
                return;
            }
            audioSource.UnPause();
        }
    }
    void playRandomSound()
    {
        randomSound = getRandomSound();
        SM.PlayOneShotSFX(randomSound, audioSource);
    }

    string getRandomSound()
    {
        int randomIndex = Random.Range(0, soundList.Length);
        return soundList[randomIndex];
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.FindWithTag("Player"))
        {
            if (soundType == SoundType.Triggered)
            {
                SM.PlayOneShotSFX(singleSound, audioSource);
            }
        }
    }
    public IEnumerator musicPattern()
    {
        randomSound = null;
        randomSound = getRandomSound();
        SM.selectMusic(randomSound, audioSource);
        StartCoroutine(SM.fadeMusicIn(audioSource, 1f));
        yield return new WaitForSeconds(audioSource.clip.length - 1f);
        StartCoroutine(SM.fadeMusicOut(audioSource));
    }

}
public enum SoundType
{
    Loop,
    OneShot,
    Random,
    Triggered,
    Music
}
