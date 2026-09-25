using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("World Space Canvas")]
    public GameObject tutCanvas;

    [Header("Audio")]
    public AudioSource audioSource;

    private bool playerNearby = false;
    private bool isPlaying = false;

    void Start()
    {
        tutCanvas.SetActive(false);
    }

    public void PlayInteraction()
    {
        if (isPlaying)
        {
            return;
        }
        isPlaying = true;
        tutCanvas.SetActive(true);
        SoundManager.current.PlayOneShotSFX("TutorialInfo", audioSource);
        StartCoroutine(WaitForAudioToFinish());

    }
    IEnumerator WaitForAudioToFinish()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        isPlaying = false;
        tutCanvas.SetActive(false);
    }

}
