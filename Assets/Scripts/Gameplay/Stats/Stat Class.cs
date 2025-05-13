using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public abstract class StatClass : MonoBehaviour
{
    [Header("Core Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float defense = 5f;
    public float movementSpeed = 5f;

    [Header("Damage Stats")]
    public float damage = 10f;
    public float critChance = 5f;
    public float critMultiplier = 2f;



    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    // Calculates damage with a Crit calculation taken into considerataion
    public float CalculateDamage()
    {
        bool isCrit = Random.Range(0f, 100f) <= critChance;
        return isCrit ? damage * critMultiplier : damage;
    }


    public abstract void TakeDamage(float incomingDamage, DamageHandler.DOTType? dotType = null);

}
