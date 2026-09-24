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

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(interactKey) && !isPlaying)
        {
            PlayInteraction();
        }

        
        if (isPlaying && !audioSource.isPlaying)
        {
            tutCanvas.SetActive(false);
            isPlaying = false;
        }
    }

    void PlayInteraction()
    {
        isPlaying = true;

        tutCanvas.SetActive(true);
        audioSource.Play();
    }

    

}
