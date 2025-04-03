using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed;
    public float moveSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float jumpStrength;
    public float jumpCooldown;
    public float airMultiplier;


    bool _canJump = true;


    [Header("Dash")]
    public float dashSpeed; 
    public float dashCooldown;
    public float dashTime;
    public bool _dashing;
    bool _canDash = true;

    private Vector3 _dashDirection;

    //[SerializeField] private TrailRenderer _trailRenderer;




    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;



    [Header("Ground Check")]
    // Should be half the height of the player and slightly more 
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask Ground;
    bool grounded;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //_trailRenderer.emitting = false;

    }

    private void Update()
    {
        GroundChecker();
        SpeedController();
        MyInput();

    }

    private void FixedUpdate()
    {

        MovePlayer();

        if (_dashing)
        {
            // Apply dash velocity while preserving vertical velocity (gravity)
            Vector3 newVelocity = _dashDirection * dashSpeed;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
        }
        else
        {
            MovePlayer();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        // Checks if you are sprinting
        if (Input.GetKey(sprintKey))
        {

           
        }

        // Checks if you can jump and is grounded
        if (Input.GetKey(jumpKey) && _canJump && grounded)
        {
            _canJump = false;

            Jump();

            // Wait for cooldown to jump again, lets you hold space to jump continiously
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Checks if you can Dash
        if (Input.GetKey(dashKey) && _canDash)
        {
            Debug.Log("PRessing left Control");

            StartCoroutine(Dash());

            // Wait for cooldown to dash again, lets you hold shift to dash continiously
            Invoke(nameof(ResetDash), dashCooldown);
        }

    }

    private void MovePlayer()
    {
        // Gets the movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;




        // Ground Controller
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }

    private void GroundChecker()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        if (grounded)
        {
            rb.drag = groundDrag;
            Debug.Log("Player is on the Ground");
        }
        else
        {
            rb.drag = 0;
            Debug.Log("Not grounded");
        }

    }
    private void SpeedController()
    {

        Vector3 baseVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (baseVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = baseVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpStrength, ForceMode.Impulse);
        Debug.Log("Jumping");
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    private IEnumerator Dash()
    {
        _canDash = false;
        _dashing = true;

        // Calculate dash direction based on input or orientation
        if (moveDirection != Vector3.zero)
        {
            _dashDirection = moveDirection.normalized;
        }
        else
        {
            _dashDirection = orientation.forward;
        }

        // Optional: Enable dash effects
        //_trailRenderer.emitting = true;

        float dashTimer = 0f;
        while (dashTimer < dashTime)
        {
            dashTimer += Time.deltaTime;
            yield return null;
        }

        _dashing = false;
        //_trailRenderer.emitting = false;

        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash()
    {
        _canDash = true;
    }

   

}
