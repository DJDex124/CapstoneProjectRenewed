using UnityEngine;

public class PlayerMovementCC : MonoBehaviour
{
    [Header("Movment Settings")]
    public float speed = 5f; 
    public float gravity = -9.81f; 
    public float jumpHeight = 1.5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public bool isCrouching = false;
    public bool isMoving = false;

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundMask;

    public CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    
    public static PlayerMovementCC current;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackRadius = 0.5f;
    public float attackDmg = 25f;
    public float attackCldwn = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyMask;

    private float nextAttackTime = 0f;

    public Animator animator;

    public TrailRenderer swingTrail;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        current = this;

        if (swingTrail != null)
            swingTrail.emitting = false;
    }

   
    void Update()
    {
        groundcheck();
        jump();
        handleSpeed();
        attack();
        

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void groundcheck()
    {  
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundMask);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + attackPoint.forward * attackRange);
        Gizmos.DrawWireSphere(attackPoint.position + attackPoint.forward * attackRange, attackRadius);
    }
    void jump()
    {
        if (GameManager.current == null)
            return;
        if (Input.GetButtonDown("Jump") && isGrounded && GameManager.current.canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jumping with velocity: " + velocity.y);
            GameManager.current.currentStamina -= 10f;
        }
    }
    void handleSpeed()
    {
        if (GameManager.current == null)
            return;

        if (Input.GetKey(KeyCode.LeftShift) && GameManager.current.canSprint && isGrounded)
        {
            isCrouching = false;
            speed = sprintSpeed;
            GameManager.current.currentStamina -= 20f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            speed = crouchSpeed;
            GameManager.current.canJump = false;
            GameManager.current.RegenerateStamina(10f * Time.deltaTime);
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
            GameManager.current.canJump = true;
            speed = 5f;
            GameManager.current.RegenerateStamina(10f * Time.deltaTime);
        }
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    

    void attack()
    {
        if (Time.time < nextAttackTime)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            nextAttackTime = Time.time + attackCldwn;

            animator.SetTrigger("Attack");

        
        }
    }

    public void PerformAttackHit()
    {

        RaycastHit[] hits = Physics.SphereCastAll(
            attackPoint.position,
            attackRadius,
            attackPoint.forward,
            attackRange,
            enemyMask
        );

        Debug.Log("Hits: " + hits.Length);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Hit object: " + hit.collider.name);

            EnemyScript enemy = hit.collider.GetComponentInParent<EnemyScript>();

            if (enemy != null)
            {
                Debug.Log("Enemy damaged");
                enemy.TakeDamage(attackDmg);
            }
        }
    }


    public void EnableTrail()
    {
        swingTrail.emitting = true;
    }

    public void DisableTrail()
    {
        swingTrail.emitting = false;
    }
}
