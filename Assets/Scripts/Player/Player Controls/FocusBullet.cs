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

    private void OnTriggerEnter(Collider other)
    {
        if (projectileController == null) return;

        // Skip if this is the source's own collider
        if (other.CompareTag(projectileController.sourceTag)) return;

        // Check for enemy hit
        if (other.CompareTag("Enemy"))
        {
            // Invoke the static event from PlayerShoot
            PlayerShoot.OnEnemyFocused?.Invoke(other.transform);
        }
    }
}