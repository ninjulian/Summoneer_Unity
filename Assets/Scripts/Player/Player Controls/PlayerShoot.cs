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

    [HideInInspector] public bool canShoot = true;
    [HideInInspector] public bool canFocus = true;

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
    public GameObject primaryBeam;
    public GameObject focusBeam;

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
            canShoot = false;
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
            //Vector3 shootDirection;
            //if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ignoreMask))
            //{
            //    shootDirection = (hit.point - muzzleTransform.position).normalized;
            //    //pdateAimTarget(hit);
            //}
            //else
            //{
            //    shootDirection = cameraTransform.forward;
            //    //UpdateAimTarget(hit);
            //}

            Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ignoreMask);
            RayscanHit(hit);


            //Debug.Log((stats == null), stats);
            

            //// Apply bullet velocity
            //rb.velocity = shootDirection * projectileController.speed;

            //// Transfers data to projectile
            //projectileController.baseDamage = playerStats.CalculateDamage();
            //projectileController.sourceTag = "Player";
            //CheckDOT(projectileController);

            CreateBeamTrail(muzzleTransform.position, hit.point, primaryBeam);

            yield return new WaitForSeconds(fireCooldown);
            canShoot = true;
        }
    }

    public IEnumerator FocusShoot(GameObject bulletChoice)
    {
        if (canFocus)
        {
            canFocus = false;
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

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ignoreMask))
            {
                //shootDirection = (hit.point - muzzleTransform.position).normalized;

                // UpdateAimTarget(hit);
            }
            else
            {
                //shootDirection = cameraTransform.forward;
            }

            RayscanHit(hit);

            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyFocused?.Invoke(hit.collider.transform);
            }
           


            // Old Projectile based shooting
            // Apply physics force
            //rb.velocity = shootDirection * projectileController.speed;

            // Configure bullet properties
            //projectileController.baseDamage = Mathf.Floor(playerStats.CalculateDamage() * 0f); // Focus mode damage multiplier
            //projectileController.sourceTag = "Player";
            //CheckDOT(projectileController);

            CreateBeamTrail(muzzleTransform.position, hit.point, focusBeam);

            // Cooldown management
            yield return new WaitForSeconds(fireCooldown * 3f);
            canFocus = true;
        }
    }

    //private void UpdateAimTarget(RaycastHit hit)
    //{
    //    aimTarget.position = hit.point;

    //}

    public void RayscanHit( RaycastHit hit)
    {
        Debug.Log("Thing you hit was " + hit.collider.gameObject.name);
        EnemyStats stats = hit.collider.GetComponent<EnemyStats>();
        DamageHandler enemyDamageHandler = hit.collider.GetComponent<DamageHandler>();

        if (stats != null || enemyDamageHandler != null)
        {
            enemyDamageHandler.ReceiveDamage(playerStats.CalculateDamage());

            // Apply DOT effects
            if (applyFireDOT)
            {
                enemyDamageHandler.ApplyFireDOT(playerStats.CalculateDamage(), 3f);
            }
            if (applyPoisonDOT)
            {
                enemyDamageHandler.ApplyPoisonDOT(playerStats.CalculateDamage(), 5f);
            }
        }
    }

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


    private void CreateBeamTrail(Vector3 start, Vector3 end, GameObject beamTrail)
    {
        GameObject trail = Instantiate(beamTrail, start, Quaternion.identity);
        LineRenderer line = trail.GetComponent<LineRenderer>();
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        Destroy(trail, 0.1f);
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
