using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private SpikeTrap trap;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the trap trigger.");
            // Add logic for what happens when the player enters the trap trigger
            StartCoroutine(trap.moveUp());
        }
    }
}
