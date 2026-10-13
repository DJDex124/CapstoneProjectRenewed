using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class MaggotEnemy : MonoBehaviour
{
    private enum State { Idle, Patrol, Chase, Attack, Dead }

    #region References
    [Header("References")]
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;
    private Transform player;

    [Header("Layers")]
    [SerializeField] private LayerMask playerMask;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float arriveDistance = 1f;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public bool isDead;

    [Header("Detection Settings")]
    [SerializeField] private float hearingRange = 15f;

    [Header("Debug")]
    [SerializeField] private State currentState = State.Idle;
    #endregion

    
    private Vector3 patrolTarget;
    private bool hasPatrolTarget;
    private float patrolWaitUntil;

    private Vector3 lastKnownPosition;
    private bool hasLastKnown;

    private float nextAttackTime;

    private bool canHearPlayer;
    private bool isPlayerInRange;

    private State animState = (State)(-1);

    private void Awake()
    {
        if (navMesh == null) navMesh = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        DetectPlayer();
        UpdateBehaviour();
    }

    #region Detection
    private void DetectPlayer()
    {
        canHearPlayer = Physics.CheckSphere(transform.position, hearingRange, playerMask);
        isPlayerInRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        
        if (canHearPlayer && player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        if (canHearPlayer && player != null)
        {
            lastKnownPosition = player.position;
            hasLastKnown = true;
        }
    }
    #endregion

    #region State logic
    private void UpdateBehaviour()
    {
        if (!navMesh.isOnNavMesh) return;

        if (isPlayerInRange && player != null)
        {
            SetState(State.Attack);
            PerformAttack();
        }
        else if (canHearPlayer && player != null)
        {
            SetState(State.Chase);
            navMesh.isStopped = false;
            navMesh.speed = chaseSpeed;
            navMesh.SetDestination(player.position);
        }
        else if (hasLastKnown)
        {
           
            SetState(State.Chase);
            navMesh.isStopped = false;
            navMesh.speed = chaseSpeed;
            navMesh.SetDestination(lastKnownPosition);

            if (HasArrived())
            {
                hasLastKnown = false;
                patrolWaitUntil = Time.time + patrolWaitTime;
            }
        }
        else
        {
            PerformPatrol();
        }
    }

    private void PerformAttack()
    {
        navMesh.isStopped = true;

        
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            animator.SetTrigger("Attacking");
            DealDamage();
        }
    }

    private void PerformPatrol()
    {
        navMesh.speed = patrolSpeed;

       
        if (Time.time < patrolWaitUntil)
        {
            navMesh.isStopped = true;
            SetState(State.Idle);
            return;
        }

        navMesh.isStopped = false;

        if (!hasPatrolTarget)
        {
            hasPatrolTarget = FindPatrolPoint(out patrolTarget);
            if (hasPatrolTarget) navMesh.SetDestination(patrolTarget);
            return; 
        }

        SetState(State.Patrol);

        
        if (HasArrived() || navMesh.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            hasPatrolTarget = false;
            patrolWaitUntil = Time.time + patrolWaitTime;
        }
    }

    private bool HasArrived()
    {
        return !navMesh.pathPending && navMesh.remainingDistance <= arriveDistance;
    }

    private bool FindPatrolPoint(out Vector3 point)
    {
        Vector3 random = transform.position + new Vector3(
            Random.Range(-patrolRadius, patrolRadius),
            0f,
            Random.Range(-patrolRadius, patrolRadius));

        if (NavMesh.SamplePosition(random, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }

        point = transform.position;
        return false;
    }
    #endregion

    #region Animation
    
    private void SetState(State newState)
    {
        currentState = newState;
        if (animState == newState) return;
        animState = newState;

        switch (newState)
        {
            case State.Idle:
                animator.SetTrigger("Idle");
                break;
            case State.Patrol:
            case State.Chase:
                animator.SetTrigger("Running");
                break;
                
        }
    }
    #endregion

    #region Combat
    private void DealDamage()
    {
        HealthStaminaSystem playerHealth = player.GetComponentInChildren<HealthStaminaSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void takeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        currentState = State.Dead;
        navMesh.isStopped = true;
        navMesh.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}