using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening; // Add DOTween namespace

public class PlayerStatUI : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text defense;
    [SerializeField] private TMP_Text damage;
    [SerializeField] private TMP_Text critChance;
    [SerializeField] private TMP_Text critMultiplier;
    [SerializeField] private TMP_Text fireRate;
    [SerializeField] private TMP_Text movementSpeed;
    [SerializeField] private TMP_Text dashStrength;
    [SerializeField] private TMP_Text pickUpRadius;
    [SerializeField] private TMP_Text affinity;

    [Header("Summling Stats")]
    [SerializeField] private TMP_Text summlingSpeed;
    [SerializeField] private TMP_Text summlingDamage;
    [SerializeField] private TMP_Text summlingCritChance;
    [SerializeField] private TMP_Text summlingCritMultiplier;
    [SerializeField] private TMP_Text summlingRange;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SummlingManager summlingManager;

    [Header("Animation Settings")]
    [SerializeField] private float pulseScaleAmount = 0.1f;
    [SerializeField] private float pulseDuration = 0.3f;

    // Store previous values to detect changes
    private Dictionary<TMP_Text, string> previousTextValues = new Dictionary<TMP_Text, string>();

    void OnEnable()
    {
        // Initialize previous values
        StoreCurrentValues();
        UpdateStats();
    }

    public void UpdateStats()
    {
        // Player stats
        UpdateStatText(health, playerStats.currentHealth.ToString() + " / " + playerStats.maxHealth.ToString());
        UpdateStatText(defense, playerStats.defense.ToString());
        UpdateStatText(damage, playerStats.damage.ToString("F1"));
        UpdateStatText(critChance, playerStats.critChance.ToString("F1") + "%");
        UpdateStatText(critMultiplier, playerStats.critMultiplier.ToString("F1") + "x");
        UpdateStatText(fireRate, playerStats.fireRate.ToString("F2"));
        UpdateStatText(movementSpeed, playerStats.movementSpeed.ToString("F2"));
        UpdateStatText(dashStrength, playerStats.dashStrength.ToString("F1"));
        UpdateStatText(pickUpRadius, playerStats.pickUpRadius.ToString("F1"));
        UpdateStatText(affinity, playerStats.affinity.ToString("F1"));

        // Summling stats
        if (summlingManager != null && summlingManager.summlingsOwned.Count > 0)
        {
            var summlingStats = summlingManager.summlingsOwned[0].GetComponent<SummlingStats>();
            UpgradeManager.Instance.ApplyExistingModifiersToSummling(summlingStats);

            UpdateStatText(summlingSpeed, summlingStats.movementSpeed.ToString("F2"));
            UpdateStatText(summlingDamage, summlingStats.damage.ToString("F1"));
            UpdateStatText(summlingCritChance, summlingStats.critChance.ToString("F1") + "%");
            UpdateStatText(summlingCritMultiplier, summlingStats.critMultiplier.ToString("F1") + "x");
            UpdateStatText(summlingRange, summlingStats.attackRange.ToString("F1"));
        }
        else
        {
            UpdateStatText(summlingSpeed, "0");
            UpdateStatText(summlingDamage, "0");
            UpdateStatText(summlingCritChance, "0%");
            UpdateStatText(summlingCritMultiplier, "0x");
            UpdateStatText(summlingRange, "0");
        }

        // Store current values for next comparison
        StoreCurrentValues();
    }

    private void UpdateStatText(TMP_Text textElement, string newValue)
    {
        // Check if value changed
        if (previousTextValues.TryGetValue(textElement, out string previousValue) &&
            previousValue != newValue)
        {
            // Value changed - update text and pulse animation
            textElement.text = newValue;
            PulseAnimation(textElement);
        }
        else
        {
            // No change - just update text
            textElement.text = newValue;
        }
    }

    private void PulseAnimation(TMP_Text textElement)
    {
        // Kill any existing animations on this object
        DOTween.Kill(textElement.transform);

        // Create pulse animation
        textElement.transform.localScale = Vector3.one;
        textElement.transform.DOPunchScale(Vector3.one * pulseScaleAmount, pulseDuration, 1, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    private void StoreCurrentValues()
    {
        // Store all current text values for comparison
        previousTextValues[health] = health.text;
        previousTextValues[defense] = defense.text;
        previousTextValues[damage] = damage.text;
        previousTextValues[critChance] = critChance.text;
        previousTextValues[critMultiplier] = critMultiplier.text;
        previousTextValues[fireRate] = fireRate.text;
        previousTextValues[movementSpeed] = movementSpeed.text;
        previousTextValues[dashStrength] = dashStrength.text;
        previousTextValues[pickUpRadius] = pickUpRadius.text;
        previousTextValues[affinity] = affinity.text;
        previousTextValues[summlingSpeed] = summlingSpeed.text;
        previousTextValues[summlingDamage] = summlingDamage.text;
        previousTextValues[summlingCritChance] = summlingCritChance.text;
        previousTextValues[summlingCritMultiplier] = summlingCritMultiplier.text;
        previousTextValues[summlingRange] = summlingRange.text;
    }
}