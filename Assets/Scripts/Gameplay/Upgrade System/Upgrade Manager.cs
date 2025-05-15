using UnityEngine;
using System.Collections.Generic;
using static UpgradeData;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Settings")]
    public float waveCostFactor = 1.2f;
    public List<UpgradeData> allUpgrades;

    [Header("Tier Weights")]
    [Range(0, 100)] public int commonWeight = 50;
    [Range(0, 100)] public int uncommonWeight = 30;
    [Range(0, 100)] public int epicWeight = 15;
    [Range(0, 100)] public int legendaryWeight = 5;

    [Header("References")]
    public Transform[] upgradeSlots;
    [SerializeField] private GameObject upgradeButtonPrefab;
    
    //Stat Components
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerShoot playerShoot;

    [SerializeField] private TMP_Text upgradeDescriptionText;
    private WaveManager waveManager;
    [SerializeField] private UpgradeUI upgradeUI;

    private List<UpgradeButton> _currentButtons = new();

    //Reroll variables
    [SerializeField] private TMP_Text rerollText;
    private float rerollCost = 1f;
    public bool hasRerolled = false;

    void Awake() => Instance = this;

    public void Start()
    {
        waveManager = GetComponent<WaveManager>();
    }

    public void GenerateUpgrades(int currentWave)
    {
        CalculateRerollCost();
        ClearExisting();

        for (int i = 0; i < 5; i++)
        {
            var upgrade = GetWeightedUpgrade();
            CreateUpgradeButton(upgrade, currentWave);
        }
    }

    UpgradeData GetWeightedUpgrade()
    {
        var weightedList = new List<UpgradeData>();

        // Create weighted list from all upgrades that haven't reached their stack limit
        foreach (var upgrade in allUpgrades)
        {
            // Skip upgrades that have reached their stack limit
            if (upgrade.stackLimit > 0 && upgrade.currentStackCount >= upgrade.stackLimit)
                continue;

            int weight = upgrade.tier switch
            {
                Tier.Common => commonWeight,
                Tier.Uncommon => uncommonWeight,
                Tier.Epic => epicWeight,
                Tier.Legendary => legendaryWeight,
                _ => 0
            };

            for (int i = 0; i < weight; i++)
                weightedList.Add(upgrade);
        }

        // Fallback system
        if (weightedList.Count == 0)
        {
            Debug.LogWarning("Weighted list empty, using fallback!");

            // First try common upgrades that haven't reached their limit
            var commonUpgrades = allUpgrades.FindAll(u => u.tier == Tier.Common &&
                                                        (u.stackLimit == 0 || u.currentStackCount < u.stackLimit));
            if (commonUpgrades.Count > 0)
                return commonUpgrades[Random.Range(0, commonUpgrades.Count)];

            // Then try any upgrade that hasn't reached its limit
            var availableUpgrades = allUpgrades.FindAll(u => u.stackLimit == 0 || u.currentStackCount < u.stackLimit);
            if (availableUpgrades.Count > 0)
                return availableUpgrades[Random.Range(0, availableUpgrades.Count)];

            // Final error if completely empty
            Debug.LogError("No upgrades available in the system!");
            return null;
        }

        return weightedList[Random.Range(0, weightedList.Count)];
    }

    public int CalculatePrice(UpgradeData upgrade, int wave)
    {
        float multiplier = upgrade.tier switch
        {
            Tier.Common => 1f,
            Tier.Uncommon => 1.2f,
            Tier.Epic => 1.5f,
            Tier.Legendary => 2f,
            _ => 1f
        };

        return Mathf.RoundToInt((upgrade.baseCost + wave * waveCostFactor) * multiplier);
    }

    public void ClearExisting()
    {
        // Create a list of buttons that are not null to avoid destroying already destroyed objects
        foreach (var button in _currentButtons.Where(b => b != null).ToList())
        {
            Destroy(button.gameObject);
        }
        _currentButtons.Clear();
    }

    public void ClearButton(GameObject buttonObj)
    {
        _currentButtons.Remove(buttonObj.GetComponent<UpgradeButton>());
        Destroy(buttonObj, 0.1f);
    }

    public void CreateUpgradeButton(UpgradeData data, int wave)
    {
        if (data == null)
        {
            Debug.LogWarning("Tried to create null upgrade button!");
            return;
        }

        GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeSlots[_currentButtons.Count]);


        Button buttonComponent = buttonObj.GetComponentInChildren<Button>();

        //Gets rid of button no matter what. Moved to OnClick() in Upgrade Button
       // buttonComponent.onClick.AddListener(() => ClearButton(buttonObj));
        

        var button = buttonObj.GetComponent<UpgradeButton>();
        button.GetDescriptionTextBox(upgradeDescriptionText);
        button.Initialize(data, CalculatePrice(data, wave));
        _currentButtons.Add(button);
    }
    public void ApplyUpgradeEffects(List<StatModifier> effects)
    {
        foreach (var effect in effects)
        {
            switch (effect.statType)
            {   

                //Player Modifiers
                //Survival
                case StatType.Health:
                    ApplyEffect(effect, ref playerStats.currentHealth);
                    break;
                case StatType.MaxHealth:
                    ApplyEffect(effect, ref playerStats.maxHealth);
                    break;
                case StatType.Defense:
                    ApplyEffect(effect, ref playerStats.defense);
                    break;

                //Damage
                case StatType.Damage:
                    ApplyEffect(effect, ref playerStats.damage);
                    break;
                case StatType.FireRate:
                    ApplyEffect(effect, ref playerStats.fireRate);
                    break;
                case StatType.CritChance:
                    ApplyEffect(effect, ref playerStats.critChance);
                    break;
                case StatType.CritMultiplier:
                    ApplyEffect(effect, ref playerStats.critMultiplier);
                    break;


                //Movement
                case StatType.MovementSpeed:
                    ApplyEffect(effect, ref playerStats.movementSpeed);
                    break;
                case StatType.JumpHeight:
                    ApplyEffect(effect, ref playerStats.jumpHeight);
                    break;
                case StatType.DashStrength:
                    ApplyEffect(effect, ref playerStats.dashStrength);
                    break;
                case StatType.DashCooldown:
                    ApplyEffect(effect, ref playerStats.dashCooldown);
                    break;

                //Quality of Life (QOL)
                case StatType.Luck:
                    ApplyEffect(effect, ref playerStats.luck);
                    break;
                case StatType.Affinity:
                    ApplyEffect(effect, ref playerStats.affinity);
                    break;
                case StatType.PickUpRadius:
                    ApplyEffect(effect, ref playerStats.pickUpRadius);
                    break;


                //Summling Modifiers
                case StatType.SummlingDamage:
                    ApplyEffect(effect, ref playerStats.defense);
                    break;
                case StatType.SummlingRange:
                    ApplyEffect(effect, ref playerStats.defense);
                    break;
                case StatType.SummlingCC:
                    ApplyEffect(effect, ref playerStats.defense);
                    break;
                case StatType.SummlingCM:
                    ApplyEffect(effect, ref playerStats.defense);
                    break;
            }
        }
    }

    private void ApplyEffect(StatModifier effect, ref float stat)
    {
        if (effect.isPercentage)
        {
            // Apply percentage and round down
            stat = Mathf.Floor(stat * (1 + effect.value));
        }
        else
        {
            // Apply flat value and round down
            stat = Mathf.Floor(stat + effect.value);
        }
    }

    public void RerollButton()
    {   
        if (playerStats.soulEssence >= rerollCost)
        {
            hasRerolled = true;

            playerStats.SpendSoulEssence(rerollCost);

            GenerateUpgrades(waveManager.currentWave);

            //UpgradeUI.Instance.UpdateCurrencyText();
        
        }

    }

    public void CalculateRerollCost()
    {

        //Base cost determined by wave
        if (!hasRerolled)
        {
            rerollCost = Mathf.Ceil(0.40f * waveManager.currentWave);
            rerollText.text = rerollCost.ToString();
        }
        else
        {   
            rerollCost = Mathf.Ceil(0.70f * waveManager.currentWave) + rerollCost;
            rerollText.text = rerollCost.ToString();
        }

       

    }

}