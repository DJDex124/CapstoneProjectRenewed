using UnityEngine;

public class soundSourceHolder : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] Sound[] soundsToUse;
    private void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }


        
    }

    public void getSound()
    {

        foreach (Sound s in soundsToUse)
        {
            s.source = source;
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.maxVolume;
            s.source.playOnAwake = false;
        }
    }
}
