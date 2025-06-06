using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // Input System
    private InputAction shootAction;
    private InputAction focusAction;
    private PlayerInput playerInput;

    // Player Stats
    private PlayerStats playerStats;

    public static System.Action<Transform> OnEnemyFocused;

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
        }
        else if (focusAction.IsPressed())
        {
            StartCoroutine(FocusShoot(bullet2Prefab));
            crosshairManager.currentCrosshairType = CrosshairManager.CrosshairType.Shooting2;
        }
        else
        {
            crosshairManager.currentCrosshairType = CrosshairManager.CrosshairType.Default;
        }
    }

    public IEnumerator ShootGun(GameObject bulletChoice)
    {

        if (canShoot)
        {
            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletChoice, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();

            // Gets bullet base damage from player stats
            projectileController.baseDamage = playerStats.CalculateDamage();
            projectileController.sourceTag = "Player";

            canShoot = false;

            // Ignore Layer 7 and 9
            int layerToIgnore1 = 7;
            int layerToIgnore2 = 9;
            int layerToIgnore3 = 11;
            int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2 | 1 << layerToIgnore3);

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ignoreMask))
            {
                //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * hit.distance, Color.red, 1f);
                projectileController.target = hit.point;
                projectileController.hit = true;
               
                //Debug.Log("hit something");
                yield return new WaitForSeconds(fireCooldown);
                canShoot = true;
            }
            else
            {
                //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * bulletMissDistance, Color.blue, 1f);
                projectileController.target = cameraTransform.position + cameraTransform.forward * bulletMissDistance;
                projectileController.hit = false;
                yield return new WaitForSeconds(fireCooldown);
                canShoot = true;
            }



        }

    }

    public IEnumerator FocusShoot(GameObject bulletChoice)
    {

        if (canShoot)
        {
            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletChoice, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();

            // Gets bullet base damage from player stats
            projectileController.baseDamage = playerStats.CalculateDamage();
            projectileController.sourceTag = "Player";

            canShoot = false;

            // Ignore Layer 7 and 9
            int layerToIgnore1 = 7;
            int layerToIgnore2 = 9;
            int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2);

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ignoreMask))
            {
                //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * hit.distance, Color.red, 1f);
                projectileController.target = hit.point;
                projectileController.hit = true;
                //Debug.Log("hit something");

                yield return new WaitForSeconds(fireCooldown);
                canShoot = true;

            }
            else
            {
                //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * bulletMissDistance, Color.blue, 1f);
                projectileController.target = cameraTransform.position + cameraTransform.forward * bulletMissDistance;
                projectileController.hit = false; // Set hit to false on miss
                //Debug.Log("nothing");
                yield return new WaitForSeconds(fireCooldown);
                canShoot = true;
            }



        }



        // Debug.Log("Tryin to shoot");



    }

    public void UpdateFireRate()
    {
        // Convert bullets-per-second to cooldown between shots
        fireCooldown = 1f / playerStats.fireRate;
    }
}
