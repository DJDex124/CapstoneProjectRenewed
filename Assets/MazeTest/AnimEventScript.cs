using UnityEngine;

public class AnimEventScript : MonoBehaviour
{
    public PlayerMovementCC player;
    public EnemyScript enemy;
    

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

    public void EnemyDealDamage()
    {
        EnemyScript enemy = GetComponentInParent<EnemyScript>();
        if (enemy != null)
        {
            enemy.DealDamage();
        }
    }
}
