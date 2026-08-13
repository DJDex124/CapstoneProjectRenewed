using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class RoachEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private NavMeshAgent navMesh;

    [Header("Layers")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask ObjectMask;

    [Header("patrol Settings")]
    [SerializeField] private float targetRadius;
    private Vector3 targetPoint;
    private bool targetPointSet;
    public bool canPatrol;
    private bool patrolDelayRunning;
    private bool stuckCheckRunning;
    private bool StuckCheck = false;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float lastAttackTime;
    private bool canAttack;
    
    private bool isStuckToPlayer;
   
    [Header("Jump Attack Settings")]
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private float jumpAttackRange = 4f;
    [SerializeField] private bool isJumping;
    [SerializeField] private Vector3 jumpTarget;


    [Header("Detection Settings")]
    [SerializeField] private float hearingRange;

    private bool isPlayerInRange;
    private bool canHearPlayer;
    public enemyState currentState;
    



    private void Awake()
    {
        canPatrol = true;
        navMesh = GetComponent<NavMeshAgent>();
        
        
    }
    private void Update()
    {
        detectPlayer();
        updateBehaviorState();
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
        canAttack = Time.time - lastAttackTime >= attackCooldown && isPlayerInRange && canHearPlayer;

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
        canAttack = true;
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

            if (!stuckCheckRunning)
            { 
                StartCoroutine(stuckCheck()); 
            }

        }
        if (Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            targetPointSet = false;
        }
        if (StuckCheck)
        {
            targetPointSet = false;
        }
        performIdleAnimation();
        Debug.Log("Patrolling");

        if (!patrolDelayRunning && !targetPointSet)
        {
            StartCoroutine(patrolDelay());
        }
    }
    private void performIdleAnimation()
    {
        // Implement idle animation logic here
        // For example, you can use an Animator component to play the idle animation
        // animator.SetTrigger("Idle");
    }

    private void performChase()
    {
        if (!navMesh.isOnNavMesh || player == null) return;
        navMesh.SetDestination(player.transform.position);
        Debug.Log("Chasing");
    }

    private IEnumerator performJumpAttack()
    {
        isJumping = true;
        lastAttackTime = Time.time;

        
        if (player == null) 
        {
            isJumping = false; yield break; 
        }

        navMesh.enabled = false; // hand off movement control entirely

        jumpTarget = player.transform.position;
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float duration = Vector3.Distance(startPos, jumpTarget) / jumpSpeed;

        while (elapsed < duration && !isStuckToPlayer)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // simple arc: lerp position + a sine bump for height
            Vector3 flatPos = Vector3.Lerp(startPos, jumpTarget, t);
            flatPos.y += Mathf.Sin(t * Mathf.PI) * 1.5f; // jump arc height
            transform.position = flatPos;

            yield return null;
        }

        // if we finished the arc without hitting the player, land normally
        if (!isStuckToPlayer)
        {
            navMesh.enabled = true;
            isJumping = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isJumping && other.gameObject == player)
        {
            StickToPlayer(other.transform);
        }
    }

    private void StickToPlayer(Transform playerTransform)
    {
        isStuckToPlayer = true;
        isJumping = false;

        // deal damage once on impact
        // playerTransform.GetComponent<PlayerHealth>().TakeDamage(jumpDamage);

        transform.SetParent(playerTransform);
        transform.localPosition = Vector3.zero; // or an offset if you don't want it centered
    }
    private void updateBehaviorState()
    {
        if (isJumping)
        {
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
}
public enum enemyState
{
    Patrol,
    Chase,
    Attack
}

