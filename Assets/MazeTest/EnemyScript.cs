using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public float health = 100f;
    private NavMeshAgent agent;
    private Transform player;
    public Animator animator;

    [Header("Damage Settings")]
    public float attackRange = 2.1f;
    public float damage = 10f;
    public float attackCooldown = 1f;

    private float lastAttackTime;

    [Header("PlayerChcek")]
    public float visionRange = 10f;
    [Range(0, 360)]
    public float visionAngle = 120f;
    public LayerMask obstacleMask;
    public bool canSeePlayer = false;
    public float memoryTime = 3f;
    private float lastSeen;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 2f;
        agent.updateRotation = false;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        lastSeen = -Mathf.Infinity;
    }


    void Update()
    {
        if (player == null) return;


        canSeePlayer = CheckPlayerInSight();
       
        if (canSeePlayer)
        {
            lastSeen = Time.time;
        }
        if (Time.time > lastSeen + memoryTime)
        {
            agent.isStopped = true;
            return;
        }
        

            Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * 5f
            );
        }
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                TriggerAttack();
                lastAttackTime = Time.time;
            }
        }
    }

    bool CheckPlayerInSight()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        
        if (distanceToPlayer > visionRange)
            return false;

        
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > visionAngle / 2f)
            return false;

        
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distanceToPlayer, obstacleMask))
        {
            return false; 
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 left = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + right * visionRange);
    }

    void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void TakeDamage(float damage)
    {
        
        health -= damage;
        Debug.Log("Enemy took damage: " + damage);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }

    public void DealDamage()
    {
        if (GameManager.current != null)
        {
            GameManager.current.TakeDamage(damage);
            Debug.Log("damaging playeer");
        }
    }
}
