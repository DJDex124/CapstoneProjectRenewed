using UnityEngine;

public class AnimEventScript : MonoBehaviour
{
    [SerializeField] PlayerMovementCC player;
 
    public void PerformAttackHit()
    {
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
