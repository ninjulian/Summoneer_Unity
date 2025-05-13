// UpgradeButton.cs
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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

    public void Awake()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    public void Initialize(UpgradeData data, int price)
    {
        upgradeData = data;
        upgradePrice = price;

        nameText.text = data.upgradeName;
        //descriptionText.text = data.descriptionText;
        priceText.text = upgradePrice.ToString();
        icon.sprite = data.icon;
        //tierBorder.color = GetTierColor(data.tier);

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

            description += $"+{valueText} {statName}\n";
        }
        descriptionText.text = description;
    }

    private string GetStatDisplayName(StatType statType)
    {
        switch (statType)
        {
            case StatType.Health: return "Health";
            case StatType.Damage: return "Damage";
            case StatType.MoveSpeed: return "Move Speed";
            case StatType.JumpHeight: return "Jump Height";
            case StatType.FireRate: return "Fire Rate";
            case StatType.Defense: return "Defense";
            case StatType.Luck: return "Luck";
            case StatType.Affinity: return "Affinity";
            default: return statType.ToString();
        }
    }


    public void HighlightBuyButton()
    {
        descriptionText.text = "Buy " + upgradeData.upgradeName + "?";
    }

    public void OnClick()
    {
        if (playerStats.soulEssence >= upgradePrice)
        {
            playerStats.soulEssence -= upgradePrice;
            UpgradeManager.Instance.ApplyUpgradeEffects(upgradeData.effects);
            UpgradeUI.Instance.UpdateCurrencyText();
            Destroy(gameObject);
        }
        else
        {
            // Add visual/audio feedback for insufficient funds
        }
    }

    Color GetTierColor(Tier tier)
    {
        return tier switch
        {
            Tier.Common => Color.gray,
            Tier.Uncommon => Color.green,
            Tier.Epic => Color.magenta,
            Tier.Legendary => Color.yellow,
            _ => Color.white
        };
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