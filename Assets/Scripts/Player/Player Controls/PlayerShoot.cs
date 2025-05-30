using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerShoot : MonoBehaviour
{
    [Header("Player Shoot Stats")]
    private float fireCooldown;

    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject bullet1Prefab;
    [SerializeField] private GameObject bullet2Prefab;

    public float bulletMissDistance = 100f;

    // Transforms
    [SerializeField] private Transform muzzleTransform;
    private Transform cameraTransform;

    [SerializeField]
    private CrosshairManager crosshairManager;

    // Player Controller
    private PlayerController playerController;

    private bool canShoot = true;
    private bool canFocus = true;

    // Input System
    private InputAction shootAction;
    private InputAction focusAction;
    private PlayerInput playerInput;

    // Player Stats
    private PlayerStats playerStats;

    //Gameplay Variables
    public static System.Action<Transform> OnEnemyFocused;
    public bool applyFireDOT;
    public bool applyPoisonDOT;


    [Header("Staff Aiming")]
    [SerializeField] private Transform aimTarget;

    [Header("Animation Variables")]
    private Animator animator;
    public bool isShooting;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions["Shoot"];
        focusAction = playerInput.actions["Focus"];

        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {   
        animator = GetComponentInChildren<Animator>();

        cameraTransform = playerController.cameraTransform;

        fireCooldown = 60f / playerStats.fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateFireRate();

    }

    public void HandleInput()
    {
        if (shootAction.IsPressed())
        {
            StartCoroutine(ShootGun(bullet1Prefab));
            crosshairManager.currentCrosshairType = CrosshairManager.CrosshairType.Shooting1;
            UpdateStaffAim();
        }
        else if (focusAction.IsPressed())
        {
            StartCoroutine(FocusShoot(bullet2Prefab));
            crosshairManager.currentCrosshairType = CrosshairManager.CrosshairType.Shooting2;
            UpdateStaffAim();
        }
        else
        {
            crosshairManager.currentCrosshairType = CrosshairManager.CrosshairType.Default;
            //aimTarget.position = muzzleTransform.position;
            animator.SetBool("IsShooting", false);
        }
    }

    public IEnumerator ShootGun(GameObject bulletChoice)
    {
        if (canShoot)
        {
            RaycastHit hit;
            GameObject bullet = Instantiate(bulletChoice, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
            Rigidbody rb = bullet.GetComponent<Rigidbody>(); // Get Rigidbody

            // Ignore Layer 7 and 9
            int layerToIgnore1 = 7;
            int layerToIgnore2 = 9;
            int layerToIgnore3 = 11;
            int layerToIgnore4 = 13;
            int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2 | 1 << layerToIgnore3 | 1 << layerToIgnore4);

            // Calculate direction
            Vector3 shootDirection;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100f, ignoreMask))
            {
                shootDirection = (hit.point - muzzleTransform.position).normalized;

                //UpdateAimTarget(hit);
            }
            else
            {
                shootDirection = cameraTransform.forward;
                //UpdateAimTarget(hit);
            }

            // Apply velocity
            rb.velocity = shootDirection * projectileController.speed;

            // Existing code for damage, cooldown, etc.
            projectileController.baseDamage = playerStats.CalculateDamage();
            projectileController.sourceTag = "Player";
            CheckDOT(projectileController);
            canShoot = false;

            yield return new WaitForSeconds(fireCooldown);
            canShoot = true;
        }
    }

    public IEnumerator FocusShoot(GameObject bulletChoice)
    {
        if (canFocus)
        {
            GameObject bullet = Instantiate(bulletChoice, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            // Calculate direction
            Vector3 shootDirection = cameraTransform.forward; // Default direction
            RaycastHit hit;

            int layerToIgnore1 = 7;
            int layerToIgnore2 = 9;
            int layerToIgnore3 = 11;
            int layerToIgnore4 = 13;
            int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2 | 1 << layerToIgnore3 | 1 << layerToIgnore4);

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100f, ignoreMask))
            {
                shootDirection = (hit.point - muzzleTransform.position).normalized;

               // UpdateAimTarget(hit);
            }

            // Apply physics force
            rb.velocity = shootDirection * projectileController.speed;

            // Configure bullet properties
            projectileController.baseDamage = Mathf.Floor(playerStats.CalculateDamage() * 0.1f); // Focus mode damage multiplier
            projectileController.sourceTag = "Player";
            CheckDOT(projectileController);

            // Cooldown management
            canFocus = false;
            yield return new WaitForSeconds(fireCooldown * 2f);
            canFocus = true;
        }
    }

    //private void UpdateAimTarget(RaycastHit hit)
    //{
    //    aimTarget.position = hit.point;

    //}

    public void UpdateFireRate()
    {
        // Convert bullets-per-second to cooldown between shots
        fireCooldown = 1f / playerStats.fireRate;
    }

    private bool CheckAffinity(float affinity)
    {
        // Ensure affinity is within valid range (0-100)
        float clampedAffinity = Mathf.Clamp(affinity, 0f, 100f);

        // Generate random number between 0-100
        float randomValue = Random.Range(0f, 100f);

        // Return true if random number is less than or equal to affinity
        return randomValue <= clampedAffinity;
    }

    private void CheckDOT(ProjectileController projectileController)
    {
        // Checks if DOT should be applied
        if (applyFireDOT && CheckAffinity(playerStats.affinity))
        {
            projectileController.applyFireDOT = true;
        }

        if (applyPoisonDOT && CheckAffinity(playerStats.affinity))
        {
            projectileController.applyPoisonDOT = true;
        }
    }

    private void UpdateStaffAim()
    {   
        if (aimTarget == null || muzzleTransform == null) return;

        animator.SetBool("IsShooting", true);

        // Raycast towards camera forward
        Vector3 rayDirection = cameraTransform.forward;

        if (Physics.Raycast(muzzleTransform.position, rayDirection, out RaycastHit hitInfo, Mathf.Infinity))
        {
            // Move aim target to hit point
            aimTarget.position = hitInfo.point;
        }
        else
        {
            // Default distance
            aimTarget.position = muzzleTransform.position + rayDirection * 100f;
        }
    }
}
