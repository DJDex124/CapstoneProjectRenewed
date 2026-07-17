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
    public EnemyState enemyState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void bugBehavior()
    {
       if (isAlive)
       {
            CheckGrounded();
            wallCheck();
            playerCheck();
            facePlayer();
            switch (enemyState)
            {
                case EnemyState.Idle:
                    bugIdle();
                    break;
                case EnemyState.Chase:
                    bugChase();
                    break;
                case EnemyState.Attack:
                    JumpAttack();
                    break;
            }
       }
    }

    //------------------------ Bug Behavior methods ------------------------

    void bugIdle()
    {
       
        if (canHearPlayer)
        {
            enemyState = EnemyState.Chase;
        }
    }

    
    void bugChase()
    {
        if (canHearPlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                enemyState = EnemyState.Attack;
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
            enemyState = EnemyState.Idle;
        }
    }


    void JumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.AddForce(direction * jumpForce, ForceMode.Impulse);
            isLeaping = true;
            lastAttackTime = Time.time;
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
        if (playerMovement.isMoving)
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
    Attack
}
