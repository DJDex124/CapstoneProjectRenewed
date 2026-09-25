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
    [SerializeField]private bool isGrounded;
    [SerializeField] private HealthStaminaSystem healthStaminaSystem;

    public Animator animatorCam;

    public Transform handSpot;
    public bool makingSound = false;
    [SerializeField] private AudioSource _audioSource;
    void Start()
    {
        controller = GetComponent<CharacterController>();

       
        if (_audioSource != null)
        {
            return;
        }
        _audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        groundcheck();
        jump();
        handleSpeed();
        SlowHeal();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isMoving && !isCrouching)
        {
            makingSound = true;
        }
        else
        {
            makingSound = false;
        }
        if (makingSound)
        {
            SoundManager.current.PlayLoop("FootSteps", _audioSource);
            
        }
        else
        {
            SoundManager.current.StopLoop(_audioSource);
        }
    }

    void groundcheck()
    {  
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundMask);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }

    void jump()
    {
        if (healthStaminaSystem == null)
            return;
        if (Input.GetButtonDown("Jump") && isGrounded && healthStaminaSystem.canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jumping with velocity: " + velocity.y);
            if (healthStaminaSystem.canLoseStamina)
            { 
                healthStaminaSystem.currentStamina -= 10f; 
            }
        }
    }
    void handleSpeed()
    {
        if (healthStaminaSystem == null)
            return;

        if (Input.GetKey(KeyCode.LeftShift) && healthStaminaSystem.canSprint && isGrounded)
        {
            isCrouching = false;
            speed = sprintSpeed;
            if (healthStaminaSystem.canLoseStamina)
            {
                healthStaminaSystem.currentStamina -= 20f * Time.deltaTime;
            }
            
        }
        else if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            speed = crouchSpeed;
            healthStaminaSystem.canJump = false;
            healthStaminaSystem.RegenerateStamina(15f * Time.deltaTime);
            isCrouching = true;
            
        }
        else
        {
            isCrouching = false;
            healthStaminaSystem.canJump = true;
            speed = 5f;
            healthStaminaSystem.RegenerateStamina(10f * Time.deltaTime);
        }
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        animatorCam.SetBool("IsWalking", isMoving);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;
        animatorCam.SetBool("IsRunning", isRunning);
    }
    

    
    void SlowHeal()
    {
        if (healthStaminaSystem == null)
            return;
        if (!isMoving)
        {
            healthStaminaSystem.healPlayer(0.5f * Time.deltaTime);
        } 


    }

}
