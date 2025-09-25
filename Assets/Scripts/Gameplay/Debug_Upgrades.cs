using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Upgrades : MonoBehaviour
{
    UpgradeInventory uprgadeInventory;
    UpgradeManager upgradeManager;

    public static Debug_Upgrades Instance;

    private void Start()
    { 
        upgradeManager = FindAnyObjectByType<UpgradeManager>();
        uprgadeInventory = FindAnyObjectByType<UpgradeInventory>(); 
    }

    

    // Add Upgrade Logic, string name, int count
    public void AddUpgrade(string upgradeName, int count)
    {
        // Find the matching upgrade first
        UpgradeData matchingUpgrade = null;
        foreach (var upgrade in UpgradeManager.Instance.allUpgrades)
        {
            if (upgrade.upgradeName == upgradeName)
            {
                matchingUpgrade = upgrade;
                break;
            }
        }

        // If we found the upgrade, apply it count times
        if (matchingUpgrade != null)
        {
            for (int i = 0; i < count; i++)
            {
                uprgadeInventory.AddUpgrade(upgradeName);
                upgradeManager.ApplyUpgradeEffects(matchingUpgrade.effects);
                Debug.Log($"Added Upgrade {matchingUpgrade.upgradeName} ({i + 1}/{count})");
            }
        }
        else
        {
            Debug.LogWarning($"Upgrade '{upgradeName}' not found in allUpgrades list");
        }
    }

    // Remove Upgrade Logic, string name, int count
    public void RemoveUpgrade(string upgradeName, int count)
    {
        uprgadeInventory.RemoveUpgrade(upgradeName, count);
    }
}
