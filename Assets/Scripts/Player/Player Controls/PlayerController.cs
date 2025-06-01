using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private float playerSpeed;
    private float jumpHeight;
    public float gravityValue = -9.81f;
    private Vector3 movementDir; // Movement Direction

    [Header("Dash")]
    public float dashTimer = 0.2f;
    //private float dashCooldown = 1.5f;
    public bool canDash = true;


    [Header("Player Inputs")]
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction jumpAction;
    [HideInInspector] public InputAction dashAction;
    [HideInInspector] public InputAction shootAction;
    [HideInInspector] public InputAction focusAction;
    [HideInInspector] public InputAction pauseAction;
    [HideInInspector] public InputAction systemAction;

    //[SerializeField]
    //private Transform bulletParent;
    //private bool canShoot = true;
    //private bool isHitScan;
    //public float FireRate;

    [Header("Other")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float shootingRotationSpeed = 10f;
    [SerializeField] private TrailRenderer trailRenderer;
    private PlayerStats playerStats;
    private PlayerShoot shoot;

    private CharacterController controller;
    [HideInInspector]
    public  Transform cameraTransform;
    private Vector3 playerVelocity;
    private bool groundedPlayer;


    [Header("Animation Variables")]
    private Animator animator;
    public bool isWalking;
    public bool isDashing;
    public bool isShooting;
    public bool isFocusing;
    public bool isFalling;



    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        cameraTransform = Camera.main.transform;
        playerStats = GetComponent<PlayerStats>();
        shoot = GetComponent<PlayerShoot>();

        // Initiatlise Trail renderer
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        shootAction = playerInput.actions["Shoot"];
        focusAction = playerInput.actions["Focus"];
        systemAction = playerInput.actions["System"];
        pauseAction = playerInput.actions["Pause"];

    }

   

    void Update()
    {
        HandleGrounding();
        HandleMovement();
        HandleJump();
        HandleShootingRotation();
        ApplyGravity();


        // Update animation booleans
        bool isMoving = moveAction.ReadValue<Vector2>().magnitude > 0.1f;
        animator.SetBool("IsWalking", isMoving && groundedPlayer && !isDashing);
        
       // animator.SetBool("IsFalling", isFalling);  // NEW
       // Debug.Log("Is the player falling" + isFalling);
        animator.SetBool("IsGrounded", groundedPlayer);
      

        // Check for dash input
        if (dashAction.triggered && canDash && !isDashing)
        {
            StartCoroutine(HandleDash());
        }
    }

    private void HandleMovement()
    {
        //Gets movement input
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        //// Takes into consideration the direction of the camera when moving
        //move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        //move.y = 0f;

        // Get horizontal camera direction
        Vector3 horizontalForward = cameraTransform.forward;
        horizontalForward.y = 0;
        if (horizontalForward.sqrMagnitude > 0.01f)
        {
            horizontalForward.Normalize();
        }
        else
        {
            // If Player is looking up or down, get default forward
            horizontalForward = Vector3.forward;
        }

        Vector3 horizontalRight = cameraTransform.right;
        horizontalRight.y = 0;
        if (horizontalRight.sqrMagnitude > 0.01f)
        {
            horizontalRight.Normalize();
        }
        else
        {
            // Fallback to a default right if needed
            horizontalRight = Vector3.right;
        }

        // Calculate move direction using horizontal vectors
        move = move.x * horizontalRight + move.z * horizontalForward;
        move.y = 0f; // Ensures no verticality is taken into consideration

        movementDir = move;

        //Get movementspeed Stat
        playerSpeed = playerStats.movementSpeed; 

        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Only rotate with movement when not shooting
        if (!shootAction.IsPressed() && !focusAction.IsPressed() && move != Vector3.zero)
        {
            SmoothRotation(move, rotationSpeed);
        }


        //// Set snapping rotation of the player
        //if (move != Vector3.zero)
        //{
        //    gameObject.transform.forward = move;

        //}

    }

    private void HandleJump()
    {
        // Makes the player jump
        if (jumpAction.triggered && groundedPlayer)
        {

            animator.SetTrigger("Jump");
            isFalling = false;
            //animator.SetBool("IsJumping", isJumping);
            jumpHeight = playerStats.jumpHeight;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            Debug.Log("JUmping");
        }
       
    }


    private void HandleShootingRotation()
    {

        // Want to have this for when the player is shooting
        // Rotate towards the camera direction 
        // Rotates around the y axis

        if (shootAction.IsPressed())
        {
            // Get camera's forward direction without vertical component
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;

            SmoothRotation(cameraForward, shootingRotationSpeed);
        }
        else if (focusAction.IsPressed())
        {
            // Get camera's forward direction without vertical component
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            SmoothRotation(cameraForward, shootingRotationSpeed);
        }
    }

    private void HandleGrounding()
    {
        groundedPlayer = controller.isGrounded;

        // Falling detection
        if (!groundedPlayer && playerVelocity.y < 0)
        {
            isFalling = true;
        }
        else if (groundedPlayer)
        {
            // Reset states when landing
            isFalling = false;

        }

        animator.SetBool("IsFalling", isFalling);

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
    }

    private void SmoothRotation(Vector3 direction, float speed)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isDashing) return;

        // Add falling detection when moving downward
        if (!groundedPlayer && playerVelocity.y < 0)
        {
            isFalling = true;
        }


        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private IEnumerator HandleDash()
    {
        canDash = false;
        isDashing = true;

        //// Store original speed and gravity
        //float originalSpeed = playerSpeed;
        //float originalGravity = gravityValue;

        //// Set dash parameters
        //playerSpeed = dashStrength;
        ////gravityValue = 0;
        //playerVelocity.y = 0f;

        Vector3 dashDirection = transform.forward;
        float startTime = Time.time;

        
        trailRenderer.enabled = true;
        animator.SetBool("IsDashing", true);

        // Dash movement
        while (Time.time < startTime + dashTimer)
        {

            trailRenderer.time = dashTimer * 2f;

            // Calculates dash direction
            if (movementDir != Vector3.zero)
            {

                controller.Move(movementDir * playerStats.dashStrength * Time.deltaTime);
                //SmoothRotation(movementDir, rotationSpeed);

                yield return null;
            }
            else
            {   // If player is not moving the dash direction will go toward forward direction of player

                movementDir = transform.forward;
                //SmoothRotation(movementDir, rotationSpeed);
            }



        }

        

        trailRenderer.enabled = false;

        // Reset parameters
        //playerSpeed = originalSpeed;
        //gravityValue = originalGravity;
        isDashing = false;
        animator.SetBool("IsDashing", false);
        // Cooldown
        yield return new WaitForSeconds(playerStats.dashCooldown);
        canDash = true;

    }



}

