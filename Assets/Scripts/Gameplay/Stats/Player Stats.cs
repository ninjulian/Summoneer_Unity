using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerStats : StatClass
{
    public float focusDuration = 5f;
    public float fireRate;

    [Header("Movement")]
    public float jumpHeight;
    public float dashStrength = 10f;
    public float dashCooldown = 0.2f;

    [Header("Quality of Life (QOL)")]
    public float luck;
    public float affinity;
    // public float coolDown;
    public float pickUpRadius;

    [Header("Soul Essence")]
    public float soulEssence = 0f;
    [HideInInspector] public float sePickUpRate = 1f;

    [Header("XP")]
    public XpBar xpBar;
    [HideInInspector] public float xpRequired;
    [HideInInspector] public float currentXP;
    [HideInInspector] public float xpGainRate;
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
        currentHealth -= incomingDamage;
    }

    public void GainSoulEssence(float soulEssenceGained)
    {
        // Check for luck bonus
        float bonusMultiplier = CheckLuckBonus() ? 2f : 1f;
        soulEssence += soulEssenceGained * sePickUpRate * bonusMultiplier;
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

    // In PlayerStats.cs
    public void CalculateXPCap()
    {
        xpRequired = (playerLevel + 3f) * playerLevel;
        xpBar.xpSlider.maxValue = xpRequired; // Force UI update
    }


    public void GainXP(float incomingXP)
    {
        // Check for luck bonus
        float bonusMultiplier = CheckLuckBonus() ? 2f : 1f;
        currentXP += incomingXP * bonusMultiplier;

        // Handle potential multiple level-ups
        while (currentXP >= xpRequired)
        {
            playerLevel++;
            currentXP -= xpRequired;
            xpBar.xpSlider.value = currentXP;
            CalculateXPCap(); // Update requirement for next level
        }

        xpBar.UpdateXPBar();
    }

    private bool CheckLuckBonus()
    {
        if (luck <= 0) return false;

        // Chance equals luck percentage (e.g., 15 luck = 15% chance)
        float randomValue = Random.Range(0f, 100f);
        return randomValue <= Mathf.Clamp(luck, 0f, 100f);
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
