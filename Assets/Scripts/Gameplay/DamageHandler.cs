using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class DamageHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    public void TakeDamage(float incomingDamage)
    {   
        // Makes sure 
        float damageTaken = Mathf.Max(incomingDamage * (100/100 + playerStats.defense), 0);

        playerStats.currentHealth -= damageTaken;
        Debug.Log($"Player took {damageTaken} damage!");

        

        if (playerStats.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // Add death logic here
    }
}
