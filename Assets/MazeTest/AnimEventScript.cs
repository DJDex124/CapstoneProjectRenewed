using UnityEngine;

public class AnimEventScript : MonoBehaviour
{
    [SerializeField] PlayerMovementCC player;
 
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
