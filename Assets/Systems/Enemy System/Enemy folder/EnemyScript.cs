using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public float health = 100f;
    private NavMeshAgent agent;
    private Transform player;
    public Animator animator;
    public EnemyState currentState;

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
    public float memoryTime = 3f;
    private float lastSeen;
    private bool canSeePlayer;

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
        ChangeState(EnemyState.Idle);
    }


    void Update()
    {
        if (currentState == EnemyState.Dead || player == null) return;


        canSeePlayer = CheckPlayerInSight();
       
        if (canSeePlayer)
        {
            lastSeen = Time.time;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;
        }
    }

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    void UpdateIdle()
    {
        agent.isStopped = true;
        animator.SetBool("IsMoving", false);

        if (Time.time <= lastSeen + memoryTime)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }


    void UpdateChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        RotateTowardsPlayer();

        if (Time.time > lastSeen + memoryTime)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("IsMoving", true);
    }

    void UpdateAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        RotateTowardsPlayer();

        agent.isStopped = true;
        animator.SetBool("IsMoving", false);

        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
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

    //void TriggerAttack()                              
    //{
        //if (animator != null)
        //{
            //animator.SetTrigger("Attack");
        //}
    //}

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
