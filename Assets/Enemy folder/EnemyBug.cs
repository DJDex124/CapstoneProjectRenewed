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
    public float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Player Check")]
    public float Range = 10f;
    public bool canSeePlayer;

    [Header("Jump Attack")]
    public float jumpForce = 5f;
    public bool isLeaping = false;


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
    }

    void Update()
    {
        if (player == null) return;
        if (isLeaping) return; 

        bool inAttackRange = !agent.pathPending
                          && agent.hasPath
                          && agent.remainingDistance <= agent.stoppingDistance;

        if (inAttackRange)
            AttackPlayer();
        else
            ChasePlayer();
    }

    void ChasePlayer()
    {
        if (!agent.enabled) return; 
        agent.SetDestination(player.position);
        RotateTowardsPlayer();

    }

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

        lastAttackTime = Time.time;
        isLeaping = true;

        Debug.Log("Leaping towards player!");
    }
    void attachedToPlayer(GameObject Player)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        this.enabled = false; // disables this script's Update loop

        transform.SetParent(Player.transform);
        // deal damage to player over time as its attached.
    }

    void disAttachFromPlayer()
    {
        transform.SetParent(null);
        agent.enabled = true;
        GetComponent<Collider>().enabled = true;
        this.enabled = true; 
    }
    void OnCollisionEnter(Collision col)
    {
        if (!isLeaping) return;

        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player!");
            isLeaping = false;
            ResetToNavMesh();
            attachedToPlayer(Player);
        }
        else if (col.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Missed, hit ground.");
            isLeaping = false;
            ResetToNavMesh();
            attackCooldown += 1f;
        }
    }

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

}

