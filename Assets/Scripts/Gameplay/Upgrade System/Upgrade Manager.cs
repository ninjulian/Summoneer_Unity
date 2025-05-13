using UnityEngine;
using System.Collections.Generic;
using static UpgradeData;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Settings")]
    public int waveCostFactor = 10;
    public List<UpgradeData> allUpgrades;

    [Header("Tier Weights")]
    [Range(0, 100)] public int commonWeight = 50;
    [Range(0, 100)] public int uncommonWeight = 30;
    [Range(0, 100)] public int epicWeight = 15;
    [Range(0, 100)] public int legendaryWeight = 5;

    [Header("References")]
    public Transform[] upgradeSlots;
    public GameObject upgradeButtonPrefab;
    public PlayerStats playerStats;

    [SerializeField] private TMP_Text upgradeDescriptionText;
    private WaveManager waveManager;
    [SerializeField] private UpgradeUI upgradeUI;

    private List<UpgradeButton> _currentButtons = new();

    void Awake() => Instance = this;

    public void Start()
    {
        waveManager = GetComponent<WaveManager>();
    }

    public void GenerateUpgrades(int currentWave)
    {
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

        // Create weighted list from all upgrades
        foreach (var upgrade in allUpgrades)
        {
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

            // First try common upgrades
            var commonUpgrades = allUpgrades.FindAll(u => u.tier == Tier.Common);
            if (commonUpgrades.Count > 0)
                return commonUpgrades[Random.Range(0, commonUpgrades.Count)];

            // Then try any upgrade
            if (allUpgrades.Count > 0)
                return allUpgrades[Random.Range(0, allUpgrades.Count)];

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
        if (gameObject != null)
        { 
            foreach (var button in _currentButtons)
            Destroy(button.gameObject);
            _currentButtons.Clear();
        }
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
        buttonComponent.onClick.AddListener(() => ClearButton(buttonObj));
        

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
                case StatType.Health:
                    ApplyEffect(effect, ref playerStats.currentHealth);
                    break;
                case StatType.Damage:
                    ApplyEffect(effect, ref playerStats.damage);
                    break;
                case StatType.MoveSpeed:
                    ApplyEffect(effect, ref playerStats.movementSpeed);
                    break;
                case StatType.FireRate:
                    ApplyEffect(effect, ref playerStats.fireRate);
                    break;
                case StatType.Defense:
                    ApplyEffect(effect, ref playerStats.defense);
                    break;
                    // Add other cases as needed
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
        GenerateUpgrades(waveManager.currentWave);
    }

}