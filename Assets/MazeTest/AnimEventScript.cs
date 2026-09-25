using UnityEngine;

public class AnimEventScript : MonoBehaviour
{
    [SerializeField] PlayerInteractions player;
    private void Start()
    {
        if (player == null)
        {
            player = GetComponentInParent<PlayerInteractions>();
            if (player == null)
            {
                Debug.LogWarning("Player reference is not set in AnimEventScript and could not be found in parent.");
            }
        }
    }

    public void PerformAttackHit()
    {
        Debug.Log("Attack hit performed.");
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set in AnimEventScript.");
            return;
        }
        player.PerformAttackHit();
        
    }

    public void EnableTrail()
    {
        player.EnableTrail();
    }

    public void DisableTrail() 
    {
        player.DisableTrail();
    }
}
