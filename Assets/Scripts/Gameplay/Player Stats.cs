using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : StatClass 
{
    [Header("Player Specific")]
    public float jumpHeight;
    public float luck;
    public float affinity;
    public float focusDuration = 5f;
    public float fireRate;
    public float coolDown;
    public float dashStrength = 10f;
    private DamageHandler damageHandler;

    private void Awake()
    {   
        base.Start();
        damageHandler = GetComponent<DamageHandler>();
        damageHandler.Initialize(this);

    }

    public override void TakeDamage(float incomingDamage, DamageHandler.DOTType? dotType = null)
    {
        currentHealth -=incomingDamage;
    }

    //public float GetDamage()
    //{
    //    float damageDealt;
    //    bool isCritical = Random.Range(0f, 100f) <= critChance;

    //    if (!isCritical)
    //    {
    //        return damage;
    //    }
    //    else
    //    {
    //        damageDealt = critMultiplier * damage;
    //        return damageDealt;
    //    }
    //}


}
