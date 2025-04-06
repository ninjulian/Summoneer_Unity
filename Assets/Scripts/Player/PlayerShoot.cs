using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Player Shoot")]
    private InputAction shootAction;
    private InputAction focusAction;
    private PlayerInput playerInput;
    public float fireRate;

    [Header("Bullet Prefabs")]
    [SerializeField]
    private GameObject bullet1Prefab;
    [SerializeField]
    private GameObject bullet2Prefab;

    public float bulletMissDistance = 100f;

    [SerializeField]
    private Transform muzzleTransform;
    [SerializeField]
    private Transform cameraTransform;

    private PlayerController playerController;

    private bool canShoot = true;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions["Shoot"];
        focusAction = playerInput.actions["Focus"];
    }

    void Start()
    {
        cameraTransform = playerController.cameraTransform;
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
            canShoot = false;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * hit.distance, Color.red, 1f);
                projectileController.target = hit.point;
                projectileController.hit = true;
                Debug.Log("hit something");
                yield return new WaitForSeconds(fireRate);
                canShoot = true;
            }
            else
            {
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * bulletMissDistance, Color.blue, 1f);
                projectileController.target = cameraTransform.position + cameraTransform.forward * bulletMissDistance;
                projectileController.hit = true;
                Debug.Log("nothing");
                yield return new WaitForSeconds(fireRate);
                canShoot = true;
            }
            
        }

        Debug.Log("Tryin to shoot");

    }
}
