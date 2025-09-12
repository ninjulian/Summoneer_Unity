using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerStats : StatClass
{

    // Player speciic 
    public float focusDuration = 5f;
    public float fireRate;

    [Header("Movement")]
    public float jumpHeight;
    public float dashStrength = 10f;
    public float dashCooldown = 0.5f;

    [Header("Quality of Life (QOL)")]
    public float luck;
    public float affinity;
    // public float coolDown;
    public float pickUpRadius;

    [Header("Soul Essence")]
    public float soulEssence = 0f;
    [HideInInspector] public float sePickUpRate = 1f;

    //Not completely implemented
    [Header("XP")]
    public XpBar xpBar;
    [HideInInspector] public float xpRequired;
    [HideInInspector] public float currentXP;
    [HideInInspector] public float xpGainRate;
    public int playerLevel { get; private set; } = 1;

    private DamageHandler damageHandler;

    public UIManager uiManager;

    //Command Variables
    public bool isGodMode = false;
    private float savedMaxHealth;
    private float savedCurrentHealth;

    private void Awake()
    {
        // Calls the start function from StatClass parent class
        base.Start();
        damageHandler = GetComponent<DamageHandler>();

        // linking this object to dmage handler
        damageHandler.Initialize(this);


        //Start working on the Player XP system
    }

    private void OnDestroy()
    {
        if (currentHealth <= 0)
        {

            if (uiManager.deathScreen != null)
            {
                // Loads death screen
                uiManager.deathScreen.SetActive(true);
            }

            if (uiManager.playerHud != null)
            {
                // Hides hud
                uiManager.playerHud.SetActive(false);
            }

            // Unlock cursor so not stuck
            uiManager.UpdateCursorState();
        }


    }

    public override void TakeDamage(float incomingDamage, DamageHandler.DOTType? dotType = null)
    {
        if (isGodMode)
        {
            Debug.Log("Damage prevented by God Mode");
            return; // Skip damage processing if in god mode
        }

        currentHealth = Mathf.Clamp(currentHealth - incomingDamage, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            //Debug.Log("DEAADAD");
            // Not actually used cause I just have a restart button instead
        }

    }

    public void GainSoulEssence(float soulEssenceGained)
    {
        // Check for luck bonus, if lucky double soul essence gained
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
        // Force UI update
        xpBar.xpSlider.maxValue = xpRequired;
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

            // Update requirement for next level
            CalculateXPCap();
        }

        //Not fully incremented
        xpBar.UpdateXPBar();
    }

    private bool CheckLuckBonus()
    {
        if (luck <= 0) return false;

        // Chance equals luck percentage (e.g., 15 luck = 15% chance)
        float randomValue = Random.Range(0f, 100f);
        return randomValue <= Mathf.Clamp(luck, 0f, 100f);
    }


    // Debug Commands

    public void AddSoulEssence(float soulEssenceGained)
    {
        soulEssence += soulEssenceGained;
    }

    public void ToggleGodMode(bool val)
    {

        if (val)
        {
            savedCurrentHealth = currentHealth;
            savedMaxHealth = maxHealth;

            maxHealth = float.PositiveInfinity;
            currentHealth = float.PositiveInfinity;
        }
        else
        {
            currentHealth = savedCurrentHealth;
            maxHealth = savedMaxHealth;
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
