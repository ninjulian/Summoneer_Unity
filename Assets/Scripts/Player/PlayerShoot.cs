using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Player Shoot Stats")]
    public float fireRate;
    public float fireCooldown;

    [Header("Bullet Prefabs")]
    [SerializeField]
    private GameObject bullet1Prefab;
    [SerializeField]
    private GameObject bullet2Prefab;

    public float bulletMissDistance = 100f;

    // Transforms
    [SerializeField]
    private Transform muzzleTransform;
    [SerializeField]
    private Transform cameraTransform;

    // Player Controller
    private PlayerController playerController;

    private bool canShoot = true;

    // Input System
    private InputAction shootAction;
    private InputAction focusAction;
    private PlayerInput playerInput;

    // Player Stats
    private PlayerStats playerStats;


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

        fireCooldown = 60f / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        if (shootAction.IsPressed())
        {
            StartCoroutine(ShootGun(bullet1Prefab));
        }
        else if (focusAction.IsPressed())
        {
            StartCoroutine(ShootGun(bullet2Prefab));
        }
    }

    public IEnumerator ShootGun(GameObject bulletChoice)
    {

        if (canShoot)
        {
            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletChoice, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();

            // Gives bullet base damage from player stats
            projectileController.baseDamage = playerStats.CalculateDamage();
            projectileController.sourceTag = "Player";

            canShoot = false;

            // Ignore Layer 7 and 9
            int layerToIgnore1 = 7;
            int layerToIgnore2 = 9;
            int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2);

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ignoreMask))
            {
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * hit.distance, Color.red, 1f);
                projectileController.target = hit.point;
                projectileController.hit = true;
                //Debug.Log("hit something");
                yield return new WaitForSeconds(fireCooldown);
                canShoot = true;
            }
            else
            {
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * bulletMissDistance, Color.blue, 1f);
                projectileController.target = cameraTransform.position + cameraTransform.forward * bulletMissDistance;
                projectileController.hit = true;
                //Debug.Log("nothing");
                yield return new WaitForSeconds(fireCooldown);
                canShoot = true;
            }
            
        }

       // Debug.Log("Tryin to shoot");

    }
}
