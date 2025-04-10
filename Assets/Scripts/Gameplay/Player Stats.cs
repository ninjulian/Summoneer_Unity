using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : StatClass 
{
    [Header("Player Specific")]

    public float luck;
    public float affinity;

    private DamageHandler damageHandler;

    private void Awake()
    {   
        base.Start();
        damageHandler = GetComponent<DamageHandler>();
        damageHandler.Initialize(this);

    }

    public override void TakeDamage(float incomingDamage)
    {
        damageHandler.ReceiveDamage(incomingDamage);
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
