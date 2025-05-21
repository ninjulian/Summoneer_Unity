using UnityEngine;
using System;

public class FocusBullet : MonoBehaviour
{
    // Reference to ProjectileController for stats
    private ProjectileController projectileController;

    private void Start()
    {
        projectileController = GetComponent<ProjectileController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (projectileController == null) return;

        // Skip if this is the source's own collider
        if (collision.gameObject.CompareTag(projectileController.sourceTag)) return;

        // Check for enemy hit
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Invoke the static event from PlayerShoot
            PlayerShoot.OnEnemyFocused?.Invoke(collision.transform);
        }
    }

}