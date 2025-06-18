using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using DG.Tweening;

public class UpgradeButton : MonoBehaviour
{
    [Header("References")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    private TMP_Text descriptionText;
    public Image tierBorder;

    private UpgradeData upgradeData;
    private int upgradePrice;

    private PlayerStats playerStats;
    private UpgradeButtonAnimation upgradeButtonAnimation;

    public void Awake()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        upgradeButtonAnimation = GetComponent<UpgradeButtonAnimation>();

        upgradeButtonAnimation.SpawnItem();
    }

    public void Initialize(UpgradeData data, int price)
    {
        upgradeData = data;
        upgradePrice = price;

        nameText.text = data.upgradeName;
        //descriptionText.text = data.descriptionText;
        priceText.text = upgradePrice.ToString();
        icon.sprite = data.icon;
        tierBorder.color = GetTierColor(data.tier);

        // Finds the Button component and add the click listener
        Button button = GetComponentInChildren<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogWarning("UpgradeButton script is missing a Button component!");
        }
        
    }

    public void GetDescriptionTextBox(TMP_Text text)
    {
        descriptionText = text;
    }

    public void HighlightUpgrade()
    {
        string description = "";
        foreach (var effect in upgradeData.effects)
        {
            string statName = GetStatDisplayName(effect.statType);
            string valueText = effect.isPercentage
                ? $"{effect.value * 100:0.#}%"
                : $"{effect.value:0.#}";

            description += statName + " " + valueText + Environment.NewLine;
        }
        descriptionText.text = description;

        upgradeButtonAnimation.HoverScale();

    }

    public void NotHighlightingUpgrade()
    {
        descriptionText.text = "Welcome to the Shop";
        upgradeButtonAnimation.LeaveScale();
    }

    private string GetStatDisplayName(StatType statType)
    {
        switch (statType)
        {   
            //Player Modifiers
            case StatType.Health: return "Health";
            case StatType.Damage: return "Damage";
            case StatType.MovementSpeed: return "Move Speed";
            case StatType.FireRate: return "Fire Rate";
            case StatType.Defense: return "Defense";
            case StatType.Luck: return "Luck";
            case StatType.Affinity: return "Affinity";

            //Summling Modifiers
            case StatType.SummlingDamage: return "Summling Damage";
            case StatType.SummlingRange: return "Summling Range";
            case StatType.SummlingCC: return "Summling CC";
            case StatType.SummlingCM: return "Summling CM";
            default: return statType.ToString();
        }
    }


    public void HighlightBuyButton()
    {
        descriptionText.text = "Buy " + upgradeData.upgradeName + "?";
    }

    // Buying the Upgrade
    public void OnClick()
    {
        if (playerStats.soulEssence >= upgradePrice)
        {
            playerStats.SpendSoulEssence(upgradePrice);
            upgradeData.currentStackCount += 1;
            UpgradeManager.Instance.ApplyUpgradeEffects(upgradeData.effects);
            UpgradeUI.Instance.UpdateCurrencyText();
            StartCoroutine(DestroyUpgrade());
        }
        else
        {
            descriptionText.text = "Insufficient Soul Essence";
            //descriptionText.color = Color.red;
        }
    }

    IEnumerator DestroyUpgrade()
    {
        Button b = GetComponentInChildren<Button>();
        b.interactable = false;
        // Remove all registered events first
        EventTrigger[] triggers = GetComponentsInChildren<EventTrigger>();
        foreach (EventTrigger trigger in triggers)
        {
            // Remove all registered events first
            trigger.triggers.Clear();
            // Then destroy the component
            Destroy(trigger);
        }
        upgradeButtonAnimation.DestroyItem();
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    Color GetTierColor(Tier tier)
    {
        return tier switch
        {
            Tier.Common => Color.grey,
            Tier.Uncommon => Color.blue,
            Tier.Epic => Color.magenta,
            Tier.Legendary => Color.red,
            _ => Color.white
        };
    }

    public bool HitItemLimit()
    {
        if (upgradeData.stackLimit > 0)
        {
            upgradeData.currentStackCount += 1;

            return upgradeData.currentStackCount <= upgradeData.stackLimit;

        }

        return false;
    }


    //private void OnDestroy()
    //{
    //    Button button = GetComponent<Button>();
    //    if (button != null)
    //    {
    //        button.onClick.RemoveListener(OnClick);
    //    }
    //}
}