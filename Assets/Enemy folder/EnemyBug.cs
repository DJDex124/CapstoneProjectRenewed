using System.Collections;

using UnityEngine;
using UnityEngine.AI;


public class EnemyBug : MonoBehaviour
{
    public float health = 100f;
    private NavMeshAgent agent;
    private Transform player;
    private Rigidbody rb;
    private GameObject Player;

    [Header("Speed Settings")]
    public float speed = 3.5f;
    public float rotationSpeed = 5f;
    public float idleSpeed = 1f;
    public float wanderRadius = 5f;
    public bool isWandering = false;


    [Header("Damage Settings")]
    public float attackRange = 2.1f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Player Check")]
    public float detectionRange = 10f;
    public bool canHearPlayer = false;
    public bool playerInRange = false;

    [Header("Jump Attack")]
    public float jumpForce = 5f;
    public bool isLeaping = false;
    public float knockbackForce = 2f;

    public LayerMask groundMask;
    public EnemyState enemyState;
    public bool isAlive = true;
    public bool isGrounded;
    public float groundCheckDistance = .5f;
    private Collider collider;

    public LayerMask wallMask;
    public bool canSeeWall;
    public float wallCheckDistance = 5f;
    public bool isTurning;


    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 5f;
        agent.updateRotation = false;
        groundMask = LayerMask.GetMask("Ground");
        wallMask = LayerMask.GetMask("obstacleMask");

        collider = GetComponent<Collider>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        enemyState = EnemyState.Idle;

        
    }

    void Update()
    {
        groundcheck();
        wallCheck();
        if (player == null) return;
        if (isLeaping) return;

        canHearPlayer = !PlayerMovementCC.current.isCrouching && PlayerMovementCC.current.isMoving && playerInRange;
        bugBehavior();

    }


    //--------------------------- ENEMY STATES LOGIC------------------------------

    void bugBehavior()
    {
        bool inAttackRange = !agent.pathPending
                          && agent.hasPath
                          && agent.remainingDistance <= agent.stoppingDistance;
        if (!agent.enabled) return;

        if (inAttackRange && canHearPlayer)
        {
            AttackPlayer();
        }
        else if (playerInRange && canHearPlayer)
        {
            agent.SetDestination(player.position);
            RotateTowardsPlayer();
        } 
        else
        {
            //idleLogic();
        }
    }
    void idleLogic()
    {
        if (canSeeWall)
        {
            ResetToNavMesh();
            StartCoroutine(turnAndMove(2f));
        }
        else if (!canSeeWall && !isTurning)
        {
            agent.SetDestination(transform.position + transform.forward * 10f);
        }
        

    }
    public IEnumerator turnAndMove(float time)
    {
        isTurning = true;
        yield return new WaitForSeconds(time);
        agent.isStopped = true;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        float elapsed = 0f, duration = 0.5f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
        isTurning = false;
    }
    //--------------------------- ENEMY STATES LOGIC------------------------------


    // ---------------------------- ATTACK LOGIC ------------------ ----------
    void AttackPlayer()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        
        agent.enabled = false;
        rb.isKinematic = false;

        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 leapVelocity = dir * jumpForce;
        leapVelocity.y = jumpForce; 

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(leapVelocity, ForceMode.Impulse);

        
        isLeaping = true;

        Debug.Log("Leaping towards player!");
        lastAttackTime = Time.time;
    }
    void attachedToPlayer()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        this.enabled = false; // disables this script's Update loop

        transform.SetParent(Player.transform);
        StartCoroutine(DealDamage(1.5f));
        Debug.Log("Attached to player, will deal damage after delay.");
    }

    void disAttachFromPlayer()
    {
        transform.SetParent(null);
        agent.enabled = true;
        GetComponent<Collider>().enabled = true;
        this.enabled = true;
        ResetToNavMesh();
        lastAttackTime = Time.time;
        Debug.Log("Detached from player, resuming normal behavior.");
    }

    IEnumerator DealDamage(float delay)
    { 
        yield return new WaitForSeconds(delay);
        GameManager.current.TakeDamage(damage);
        Debug.Log("Dealing damage to player: " + damage);
        disAttachFromPlayer();
    }
    // ---------------------------- ATTACK LOGIC ------------------ ----------


    // ---------------------------- COLLISIONS AND TRIGGERS ---------------------------
    void OnCollisionEnter(Collision col)
    {
        if (!isLeaping) return;

        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player!");
            isLeaping = false;
            
            attachedToPlayer();
        }
        else if (col.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Missed, hit ground.");
            isLeaping = false;
            lastAttackTime = Time.time; 
            ResetToNavMesh();
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void Knockback()
    {
        agent.enabled = false;
        rb.isKinematic = false;
        isLeaping = false;

        Vector3 dir = (transform.position - player.position).normalized;
        dir.y = 1;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        StartCoroutine(ReEnableAgent());

    }
    IEnumerator ReEnableAgent()
    {
        if(!isAlive) yield break;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded);
        yield return new WaitForSeconds(0.1f);
        ResetToNavMesh();
    }

    void wallCheck()
    {
        float halfHeight = collider.bounds.extents.y;
        Vector3 rayOrigin = transform.position + Vector3.up *halfHeight;
        canSeeWall = Physics.Raycast(rayOrigin, rayOrigin + transform.forward, wallCheckDistance, wallMask);
    }
    
    void groundcheck()
    {
        float halfHeight = collider.bounds.extents.y;
        Vector3 rayOrigin = transform.position + Vector3.up * halfHeight;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, halfHeight + groundCheckDistance, groundMask);
    }

    private void OnDrawGizmos()
    {
        float halfHeight = collider.bounds.extents.y;
        Vector3 rayOrigin = transform.position + Vector3.up * halfHeight;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * (halfHeight + groundCheckDistance ));

        Vector3 lookDir = rayOrigin + transform.forward;
       
        Gizmos.color = canSeeWall ? Color.green : Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + lookDir * wallCheckDistance);
    }
   

    public void TakeDamage(float damage)
    {
        if (transform.parent == Player.transform)
        {
            StopAllCoroutines();
            disAttachFromPlayer();
        }
        health -= damage;
        Debug.Log("Enemy took damage: " + damage);

        if (health <= 0f)
        {
            Die();
            
        }
    }

    void Die()
    {
        Debug.Log("Enemy died.");
        agent.enabled = false; 
        this.enabled = false;
        isAlive = false;
        Destroy(gameObject, 3f);
        
    }
    // ---------------------------- COLLISIONS AND TRIGGERS ---------------------------


    //--------------------------- RESETS -------------------------
    void ResetToNavMesh()
    {
        Debug.Log("Reset Happened");
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        agent.enabled = true;
        agent.Warp(transform.position); 
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

    

    //--------------------------- RESETS -------------------------

}

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Dead
}