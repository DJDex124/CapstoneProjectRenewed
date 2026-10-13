using System.Collections;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    private AudioSource audioSource;
    HealthStaminaSystem playerSystem;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DestroyRockAfterDelay(5f)); // Destroy the rock after 5 seconds
    }

    IEnumerator DestroyRockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Destroy the rock when it collides with the player
            Destroy(gameObject);
            playerSystem = collision.gameObject.GetComponentInChildren<HealthStaminaSystem>();
            if (playerSystem != null)
            {
                playerSystem.TakeDamage(10); 
            }
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Destroy the rock when it collides with the ground
            SoundManager.current.PlayOneShotSFX("Impact", audioSource);
            
            
        }

    }
}
