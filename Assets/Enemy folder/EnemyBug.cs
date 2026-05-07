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

    [Header("Damage Settings")]
    public float attackRange = 2.1f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Player Check")]
    public float Range = 10f;
    public bool canHearPlayer = false;
    public bool playerInRange = false;

    [Header("Jump Attack")]
    public float jumpForce = 5f;
    public bool isLeaping = false;
    public float knockbackForce = 2f;

    public EnemyState enemyState;


    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 5f;
        agent.updateRotation = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        enemyState = EnemyState.Idle;
    }

    void Update()
    {
        if (player == null) return;
        if (isLeaping) return;

        if (PlayerMovementCC.current.isCrouching)
        { 
            canHearPlayer = false;
        }
        else if (PlayerMovementCC.current.isMoving)
        {
            canHearPlayer = true;
        }

        handleState();
        updateState();
        if (Input.GetKey(KeyCode.T)&& playerInRange)
        {
            Knockback();
        }    
    }


    //--------------------------- ENEMY STATES LOGIC------------------------------
    void handleState()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                agent.ResetPath();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
        }
    }
     void updateState()
    {
        bool inAttackRange = !agent.pathPending
                          && agent.hasPath
                          && agent.remainingDistance <= agent.stoppingDistance;
        if (inAttackRange)
            enemyState = EnemyState.Attack;
        else if
            (canHearPlayer && playerInRange)
            enemyState = EnemyState.Chase;
        else
            enemyState = EnemyState.Idle;


    }
    void ChasePlayer()
    {
        if (!agent.enabled) return; 
        agent.SetDestination(player.position);
        RotateTowardsPlayer();

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

    void Knockback()
    {
        agent.enabled = false;
        rb.isKinematic = false;

        Vector3 dir = (transform.position - player.position).normalized;
        dir.y = 1;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }
    // ---------------------------- COLLISIONS AND TRIGGERS ---------------------------


    //--------------------------- RESETS -------------------------
    void ResetToNavMesh()
    {
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
    Attack
}