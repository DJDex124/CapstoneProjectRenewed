using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class RoachEnemy : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private NavMeshAgent navMesh;
    public bool isDead;
    [SerializeField] private float health = 100f;
    private Vector3 directionToPlayer;
    private float distanceToPlayer;

    [Header("Layers")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask ObjectMask;

    [Header("patrol Settings")]
    [SerializeField] private float targetRadius;
    private Vector3 targetPoint;
    private bool targetPointSet;
    [SerializeField] bool canPatrol;
    private bool patrolDelayRunning;
    private bool stuckCheckRunning;
    private bool StuckCheck = false;
    [SerializeField] bool isPatrolling = false;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    private bool canAttack;
    
    private bool isStuckToPlayer;
   
    [Header("Jump Attack Settings")]
    [SerializeField] private float jumpSpeed = 4f;
    [SerializeField] private float jumpAttackRange = 4f;
    [SerializeField] private bool isJumping;
    [SerializeField] private Vector3 jumpTarget;


    [Header("Detection Settings")]
    [SerializeField] private float hearingRange;

    private bool isPlayerInRange;
    private bool canHearPlayer;
    [SerializeField] enemyState currentState;
    [SerializeField] Animator animator;
    [SerializeField] private Rigidbody rb;
    #endregion



    private void Start()
    {
        canPatrol = true;
        navMesh = GetComponent<NavMeshAgent>();
        canAttack = true;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Make Rigidbody kinematic to prevent physics interference
    }
    private void Update()
    {
        detectPlayer();
        updateBehaviorState();
        performAnimation();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, targetRadius);
    }
    private void detectPlayer()
    {
        if (isJumping)
            return;
        
        canHearPlayer = Physics.CheckSphere(transform.position, hearingRange, playerMask);
        isPlayerInRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (canHearPlayer)
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player");
            player = targetPlayer.transform;
        }
    }
    private void findPatrolPoint()
    {
        float randomX = Random.Range(-targetRadius, targetRadius);
        float randomZ = Random.Range(-targetRadius, targetRadius);

        Vector3 potentialpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(potentialpoint, -transform.up, 2f, groundMask))
        {
            targetPoint = potentialpoint;
            targetPointSet = true;
        }
        Debug.Log("Patrol point set to: " + targetPoint);
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        rb.isKinematic = true; // Make Rigidbody kinematic again after jump attack
        navMesh.enabled = true; // re-enable NavMeshAgent after jump attack
        canAttack = true;
        isJumping = false;
    }

    private void performPatrol()
    {
        if (!navMesh.isOnNavMesh) return;

        if (!targetPointSet && canPatrol)
        {
            findPatrolPoint();

        }
        if (targetPointSet)
        {
            navMesh.SetDestination(targetPoint);
            isPatrolling = true;

            if (!stuckCheckRunning)
            { 
                StartCoroutine(stuckCheck()); 
            }

        }
        if (Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            targetPointSet = false;
            isPatrolling = false;
        }
        if (StuckCheck)
        {
            targetPointSet = false;
        }
        
        Debug.Log("Patrolling");

        if (!patrolDelayRunning && !targetPointSet)
        {
            StartCoroutine(patrolDelay());
        }
    }
    private void performAnimation()
    {
        if(animator == null)
        {
            Debug.LogWarning("Animator component is not assigned.");
            return;
        }
        if (isDead)
        {
            animator.SetBool("isDead", true);
            animator.SetBool("Attack", false);
            animator.SetBool("Move", false);
            animator.SetBool("Idle", false);
            return;
        }
        if (currentState == enemyState.Attack && !isJumping)
        {
            animator.SetBool("Attack", true);
            animator.SetBool("Move", false);
            animator.SetBool("Idle", false);
        }
        else if (isPatrolling | currentState == enemyState.Chase)
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Move", true);
            animator.SetBool("Idle", false);
        }
        else 
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("Move", false);
            animator.SetBool("Idle", true);
        }

        
    }

    private void performChase()
    {
        if (!navMesh.isOnNavMesh || player == null) return;
        navMesh.SetDestination(player.transform.position);
        Debug.Log("Chasing");
        // set chase animation here
    }

    private IEnumerator performJumpAttack()
    {
        if (player == null || !navMesh)
            yield break; // Exit if the player is not found or navMesh is not available

        navMesh.enabled = false; // hand off movement control entirely
        rb.isKinematic = false; // Make Rigidbody non-kinematic to allow physics interactions
        jumpTarget = player.transform.position;
        rb.AddForce((jumpTarget - transform.position).normalized * jumpSpeed, ForceMode.Impulse);
        isJumping = true;
        StartCoroutine(AttackCooldown());

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isJumping || isStuckToPlayer)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            StickToPlayer();
        }
        else
        {
            disAttach();
        }
    }

    private void StickToPlayer()
    {
        isStuckToPlayer = true;
        
        transform.SetParent(player.transform);
        rb.isKinematic = true; // Make Rigidbody kinematic to prevent further physics interactions
        StartCoroutine(DealDamage(1.5f));
    }
    void disAttach()
    {
        isStuckToPlayer = false;
        transform.SetParent(null);
        rb.isKinematic = false; // Make Rigidbody non-kinematic to allow physics interactions
        navMesh.enabled = true; // re-enable NavMeshAgent after jump attack
        
    }
    IEnumerator DealDamage(float delay)
    {
        yield return new WaitForSeconds(delay);
        HealthStaminaSystem.current.TakeDamage(damage);
        disAttach();
    }
    private void updateBehaviorState()
    {
        if (isDead)
        {
            currentState = enemyState.Dead;
            return;
        }
        if (isJumping)
        {
            return; 
        }
        if (isStuckToPlayer)
        {
            currentState = enemyState.Attached;
            return;
        }
        if (!canHearPlayer && !isPlayerInRange)
        {
            performPatrol();
            currentState = enemyState.Patrol;
        }
        else if (canHearPlayer && !isPlayerInRange)
        {
            performChase();
            currentState = enemyState.Chase;
        }
        else if (canAttack && canHearPlayer)
        {
            StartCoroutine(performJumpAttack());
            currentState = enemyState.Attack;
        }
        
    }

    private IEnumerator patrolDelay()
    {
        patrolDelayRunning = true;
        canPatrol = false;
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before patrolling again
        canPatrol = true;
        patrolDelayRunning = false;
    }
    private IEnumerator stuckCheck()
    {
        stuckCheckRunning = true;
        StuckCheck = false;
        yield return waitForSeconds(2f);
        StuckCheck = true;
        stuckCheckRunning = false;
    }


    private IEnumerator waitForSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private void Die()
    {
        StopAllCoroutines();
        isDead = true;
        navMesh.enabled = false;
        rb.isKinematic = true; // Make Rigidbody kinematic to prevent physics interference
        currentState = enemyState.Dead;
        StartCoroutine(deathDelay(3f));
    }
    private IEnumerator deathDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    public void TakeDamage(float damage)
    {
        float knockbackForce = 5f; 
        health -= damage;
        Debug.Log("Enemy took damage: " + damage);
        navMesh.enabled = false;
        rb.isKinematic = false;
        

        Vector3 dir = (transform.position - player.position).normalized;
        dir.y = 1;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);

        if (health <= 0f)
        {
            Die();

        }
        StartCoroutine(waitForSeconds(2f));
        navMesh.enabled = true;
        rb.isKinematic = true;
    }
    void handlePlayerVisibility()
    {
        if (player == null)
            return;
       directionToPlayer = player.position - transform.position;
       distanceToPlayer = directionToPlayer.magnitude;
        if (player == null)
        {
            return;
        }
        Physics.Linecast(transform.position, player.position, out RaycastHit hit);

    }
}
public enum enemyState
{
    Patrol,
    Chase,
    Attack,
    Attached,
    Dead
}

