using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{


    [SerializeField] private ThirdPersonCam thirdplayerCam;

    public KeyCode shootKey = KeyCode.Mouse0;

    public Transform muzzleTransform; // Assign the muzzle's transform in Inspector
    public GameObject projectilePrefab;
    public float projectileSpeed = 30f;
    public float maxRayDistance = 1000f;
    public LayerMask hitLayers;
    public float FireRate;

    private bool _canShoot = true;

    void Update()
    {

        if (Input.GetKey(shootKey))
        {
            Debug.Log("Shooting");
            thirdplayerCam._isShooting = true;
            StartCoroutine(Shoot());
        }
        else
        {
            Debug.Log("Not Shooting");
            thirdplayerCam._isShooting = false;
        }


    }

    public IEnumerator Shoot()
    {

        if (_canShoot)
        {   
            _canShoot = false;

            // Get screen center (crosshair position)
            Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

            // Create ray from camera through crosshair
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            // Debugging: Draw the ray in the Scene view
            Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.red, 1.0f);

            // Determine target point
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, maxRayDistance, hitLayers))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(maxRayDistance);
            }

            // Calculate direction from muzzle to target
            Vector3 direction = (targetPoint - muzzleTransform.position).normalized;

            // Instantiate projectile
            GameObject projectile = Instantiate(projectilePrefab, muzzleTransform.position, Quaternion.LookRotation(direction));


            // Apply velocity
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            else
            {
                Debug.LogError("Projectile prefab is missing a Rigidbody component!");
            }

            yield return new WaitForSeconds(FireRate);

            _canShoot = true;

        }
    }
}

   

