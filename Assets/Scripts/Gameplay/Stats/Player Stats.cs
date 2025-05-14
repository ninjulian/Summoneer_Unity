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
    public float soulEssence = 0f;
    public int sePickUpRate;
    public float xpCap;
    public float currentXP;
    public int playerLevel { get; private set; } = 1;
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

    public void gainSoulEssence(int soulEssenceGained)
    {
        soulEssence += soulEssenceGained;
    }

    public void spendSoulEssence(int cost)
    {
        soulEssence -= cost;   
    }

    public void gainXP(float incomingXP)
    {
        currentXP += incomingXP;

        if (currentXP >= xpCap)
        {
            playerLevel++;
        }
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
