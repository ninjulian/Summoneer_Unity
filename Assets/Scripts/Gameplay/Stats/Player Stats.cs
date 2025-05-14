using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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


    //Currency
    public float soulEssence = 0f;
    [HideInInspector]public int sePickUpRate;

    //XP
    [HideInInspector]public float xpRequired;
    [HideInInspector]public float currentXP;
    [HideInInspector]public float xpGainRate;
    public int playerLevel { get; private set; } = 1;
    private DamageHandler damageHandler;

    private void Awake()
    {   
        base.Start();
        damageHandler = GetComponent<DamageHandler>();
        damageHandler.Initialize(this);
        

        //Start working on the Player XP system
    }

    public override void TakeDamage(float incomingDamage, DamageHandler.DOTType? dotType = null)
    {
        currentHealth -=incomingDamage;
    }

    public void GainSoulEssence(float soulEssenceGained)
    {
        soulEssence += soulEssenceGained * sePickUpRate;
    }

    public void SpendSoulEssence(float cost)
    {
        float newCost = Mathf.Clamp(cost, 0f, float.PositiveInfinity);

        // Check if player has enough essence
        if (soulEssence >= newCost)
        {
            soulEssence -= newCost;

        }


    }

    public void CalculateXPCap()
    {
        xpRequired = Mathf.Clamp((playerLevel + 3f) * (playerLevel), 0f, float.PositiveInfinity);
    }
    

    public void GainXP(float incomingXP)
    {
        currentXP += incomingXP;

        if (currentXP >= xpRequired)
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
