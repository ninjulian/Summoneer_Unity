// UpgradeButton.cs
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [Header("References")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text descriptionText;
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

        // Get the Button component and add the click listener
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

    public void OnClick()
    {
        if (playerStats.soulEssence >= upgradePrice)
        {
            playerStats.soulEssence -= upgradePrice;
            UpgradeManager.Instance.ApplyUpgradeEffects(upgradeData.effects);
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