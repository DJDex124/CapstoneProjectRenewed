using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float airAcceleration = 10f;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;
    public float groundDeceleration = 40f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    private void Update()
    {
        MyInput();

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.Normalize();

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 flatVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 targetVelocity = moveDirection * moveSpeed;

        float accelRate;

        if (grounded)
        {
            accelRate = (moveDirection.magnitude > 0.1f) ? acceleration : deceleration;
        }
        else
        {
            accelRate = airAcceleration;
        }

        Vector3 newFlatVelocity = Vector3.MoveTowards(
            flatVelocity,
            targetVelocity,
            accelRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(
            newFlatVelocity.x,
            currentVelocity.y,
            newFlatVelocity.z
        );
    }

    //previous tests that didnt work out but might jump back into them
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    //same as above
    private void ApplyFriction()
    {
        if (!grounded) return;

        if (horizontalInput == 0 && verticalInput == 0)
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            Vector3 newVel = Vector3.MoveTowards(
                flatVel,
                Vector3.zero,
                groundDeceleration * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector3(newVel.x, rb.linearVelocity.y, newVel.z);
        }
    }

}
