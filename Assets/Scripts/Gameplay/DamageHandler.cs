using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class DamageHandler : MonoBehaviour
{
    [SerializeField] private StatClass entityStats;

    public void ReceiveDamage(float rawDamage)
    {
        float finalDamage = Mathf.Max(rawDamage - entityStats.defense, 1);
        entityStats.currentHealth -= finalDamage;

        Debug.Log($"{gameObject.name} took {finalDamage} damage!");

        if (entityStats.currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} died!");
        // Add death logic (animation, drops, etc)
        Destroy(gameObject);
    }

    // For initialization in child classes
    public void Initialize(StatClass stats)
    {
        entityStats = stats;
    }
}
