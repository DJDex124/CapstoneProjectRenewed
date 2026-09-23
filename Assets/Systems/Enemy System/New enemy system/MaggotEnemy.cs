using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MaggotEnemy : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] private Transform player;
    private Transform lastPlayerLocation;
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
    private bool canAttack = true;

    [Header("Health settings")]
    private float currentHealth;
    private float maxHealth = 100f;

    [Header("Detection Settings")]
    [SerializeField] private float hearingRange;

    private bool isPlayerInRange;
    private bool canHearPlayer;
    [SerializeField] enemyState currentState;
    [SerializeField] Animator animator;
    
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        detectPlayer();
        updateBehaviorState();
    }
    private void updateBehaviorState()
    {
        if (isDead)
        {
            currentState = enemyState.Dead;
            return;
        }

        if (!canHearPlayer && !isPlayerInRange && lastPlayerLocation == null)
        {
            animator.SetTrigger("Idle");
            performPatrol();
            currentState = enemyState.Patrol;
        }
        else if (lastPlayerLocation != null && !isPlayerInRange)
        {
            animator.SetTrigger("Running");
            performChase();
        }
        else if (lastPlayerLocation  == null  || !isPlayerInRange)
        {
            findAttackPoint();
        }
        else if (canHearPlayer && isPlayerInRange && canAttack)
        {
            animator.SetTrigger("Attacking");
            Debug.Log("attacking");
            dealDamage();
        }
    }
    private void detectPlayer()
    {
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
    private void performChase()
    {
        if (!navMesh.isOnNavMesh || player == null) return;

        navMesh.SetDestination(lastPlayerLocation.transform.position);
        Debug.Log("Chasing");
        if (Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            lastPlayerLocation = null;
        }


    }
    private void findAttackPoint()
    {
        if (canHearPlayer)
        {
            lastPlayerLocation = player.transform;
        }

    }
    
    private void dealDamage()
    {
        
        HealthStaminaSystem playerHealth = player.GetComponentInChildren<HealthStaminaSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(20f);
        }
        Debug.Log("dealing damage");
        StartCoroutine(attackCoolDown());
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
        yield return new WaitForSeconds(2f);
        StuckCheck = true;
        stuckCheckRunning = false;
    }
    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    IEnumerator attackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    

    IEnumerator Die()
    {
        StopAllCoroutines();
        isDead = true;
        navMesh.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    
}
