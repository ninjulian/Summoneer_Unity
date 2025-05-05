using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    private Vector3 movementDir; // Movement Direction

    [Header("Dash")]
    public float dashSpeed = 10f;
    public float dashTimer = 0.2f;
    public float dashCooldown = 1f;
    public bool canDash = true;
    private bool isDashing = false;

    [Header("Player Inputs")]
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction shootAction;
    private InputAction focusAction;



    //[SerializeField]
    //private Transform bulletParent;
    //private bool canShoot = true;
    //private bool isHitScan;
    //public float FireRate;

    [SerializeField]
    private Transform muzzleTransform;

    [Header("Other")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float shootingRotationSpeed = 10f;
    [SerializeField] private TrailRenderer trailRenderer;

    private CharacterController controller;
    [HideInInspector]
    public  Transform cameraTransform;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    

    private void Awake()
    {
       
        cameraTransform = Camera.main.transform;


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
    }

   

    void Update()
    {
        HandleGrounding();

        HandleMovement();
        HandleJump();
        HandleShootingRotation();
        ApplyGravity();


        // Check for dash input
        if (dashAction.triggered && canDash)
        {
            StartCoroutine(HandleDash());
        }
    }

    private void HandleMovement()
    {
        //Gets movement input
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        // Takes into consideration the direction of the camera when moving
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;

        movementDir = move;


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
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
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
        //playerSpeed = dashSpeed;
        ////gravityValue = 0;
        //playerVelocity.y = 0f;

        Vector3 dashDirection = transform.forward;
        float startTime = Time.time;


        // Dash movement
        while (Time.time < startTime + dashTimer)
        {

            trailRenderer.enabled = true;
            trailRenderer.time = dashTimer * 2f;

            // Calculates dash direction
            if (movementDir != Vector3.zero)
            {

                controller.Move(movementDir * dashSpeed * Time.deltaTime);
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

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    //public IEnumerator ShootGun(GameObject bulletChoice)
    //{
    //    RaycastHit hit;
    //    GameObject bullet = GameObject.Instantiate(bulletChoice, muzzleTransform.position, Quaternion.identity);
    //    ProjectileController projectileController = bullet.GetComponent<ProjectileController>();

    //    if (canShoot)
    //    {
    //        canShoot = false;

    //        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
    //        {
    //            projectileController.target = hit.point;
    //            projectileController.hit = true;
    //            //Debug.Log("hit something");
    //        }
    //        else
    //        {
    //            projectileController.target = cameraTransform.position + cameraTransform.forward * 25f;
    //            projectileController.hit = false;
    //            //Debug.Log("nothing");
    //        }

    //        yield return new WaitForSeconds(FireRate);

    //        canShoot = true;
    //    }

    //    //Debug.Log("Tryin to shoot");

       
    //}


}

