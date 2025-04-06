using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction shootAction;

    [SerializeField]
    private GameObject bulletPrefab;
    private Transform bulletParent;

    [SerializeField]
    private Transform muzzleTransform;


    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {   
        cameraTransform = Camera.main.transform;

        shootAction = playerInput.actions["Shoot"];
    }


    public void ShootGun()
    {
        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.identity, bulletParent);
        ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
        //Insert Player Range
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            projectileController.target = hit.point;
            projectileController.hit = true; 
        }
        else
        {
            projectileController.target = cameraTransform.position + cameraTransform.forward * 25f;
            projectileController.hit = false;
        }

    }
}
