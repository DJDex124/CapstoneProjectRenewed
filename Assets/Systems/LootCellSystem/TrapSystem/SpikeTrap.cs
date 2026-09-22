using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 endPosition;

    [SerializeField] private GameObject player; // Reference to the player object

    private float moveSpeed = 10f; // Speed at which the spike trap moves
    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + new Vector3(0, 3, 0);
        
    }
        
        
    

    public IEnumerator moveUp()
    {
        yield return new WaitForSeconds(.5f);
        while (Vector3.Distance(transform.position, endPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            Debug.Log("Player has entered the spike trap.");
            GameManager.current.Die();
            HealthStaminaSystem healthSystem = player.GetComponent<HealthStaminaSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(100); // Assuming 100 is the damage value
            }
        }
    }
}

