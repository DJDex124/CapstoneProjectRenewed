using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class JumpBug : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private Rigidbody rb;

    [Header("Attack Settings")]
    public float attackRange = 2.1f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    public float lastAttackTime;
    public float detectionRange = 10f;
    public bool canHearPlayer = false;

    [Header("Speed Settings")]
    public float speed = 3.5f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public bool isLeaping = false;
    public float knockbackForce = 2f;
    public bool isGrounded;
    public float groundCheckDistance = .5f;

    float wallCheckDistance = 5f;
    public bool canSeeWall;

    public LayerMask groundMask;
    public LayerMask wallMask;

    bool isAlive = true;
    public BugState enemyState;

   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        bugBehavior();
    }
    public void bugBehavior()
    {
       if (isAlive)
       {
            if (canHearPlayer)
            {
                playerCheck();
            }
            CheckGrounded();
            wallCheck();
            
            facePlayer();
            switch (enemyState)
            {
                case BugState.Idle:
                    bugIdle();
                    break;
                case BugState.Chase:
                    bugChase();
                    break;
                case BugState.Attack:
                    JumpAttack();
                    break;
                case BugState.Attached:
                    break;
            }
       }
    }

    //------------------------ Bug Behavior methods ------------------------

    void bugIdle()
    {
       // need to add idle logic. need to find an idea and impliment it
        if (canHearPlayer)
        {
            enemyState = BugState.Chase;
        }
    }

    
    void bugChase()
    {
        if (canHearPlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                enemyState = BugState.Attack;
            }
            else
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            enemyState = BugState.Idle;
        }
    }


    void JumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            float upwardForce = jumpForce * 0.5f; 
            Vector3 direction = (player.transform.position - transform.position ).normalized;
            rb.AddForce(direction * jumpForce + Vector3.up * upwardForce , ForceMode.Impulse);
            isLeaping = true;
            lastAttackTime = Time.time;
        }
        if (distanceToPlayer > attackRange)
        {
            if (canHearPlayer)
            {
                enemyState = BugState.Chase;
            }
            else
            {
                enemyState = BugState.Idle;
            }
            
        }
    }
    IEnumerator DealDamage(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.current.TakeDamage(damage);
        disAttach();
    }
    void attached()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        transform.SetParent(player.transform);
        StartCoroutine(DealDamage(1.5f));
        enemyState = BugState.Attached;
    }
    void disAttach()
    {
       transform.SetParent(null);
       rb.useGravity = true;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            enemyState = BugState.Attack;
        }
        else if(canHearPlayer)
        {
            enemyState = BugState.Chase;
        }
        else
        {
            enemyState = BugState.Idle;
        }
    }
    //------------------------ Bug Behavior methods ------------------------

    //------------------------ Auto Check methods ------------------------


    void facePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    void playerCheck()
    {
        PlayerMovementCC playerMovement = player.GetComponent<PlayerMovementCC>();
        if(playerMovement.isCrouching)
        {
            canHearPlayer = false;      
        }
        else if (playerMovement.isMoving)
        {
             float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
             if (distanceToPlayer <= detectionRange)
             {
                    canHearPlayer = true;
             }
        }
        else
        {
           canHearPlayer = false;
        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(isLeaping)
            {
                attached(); 
            }
            isLeaping = false;
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            isLeaping = false;
            lastAttackTime = Time.time; 
        }
    }
    void wallCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, wallCheckDistance, wallMask))
        {
            canSeeWall = true;
        }
        else
        {
            canSeeWall = false;
        }
    }
    public void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

}
public enum BugState
{
    Idle,
    Chase,
    Attached,
    Attack
}
