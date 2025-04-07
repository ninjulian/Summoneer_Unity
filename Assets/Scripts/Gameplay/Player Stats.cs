using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealth;
    public float currentHealth;
    public float defense;
    public float movementSpeed;
    public float damage;
    public float critChance;
    public float critMultiplier;
    public float luck;
    public float affinity;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float dmgTaken)
    {
        currentHealth -= dmgTaken;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    }

    public float GetDamage()
    {
        float damageDealt;
        bool isCritical = Random.Range(0f, 100f) <= critChance;

        if (!isCritical)
        {
            return damage;
        }
        else
        {
            damageDealt = critMultiplier * damage;
            return damageDealt;
        }
    }


}
