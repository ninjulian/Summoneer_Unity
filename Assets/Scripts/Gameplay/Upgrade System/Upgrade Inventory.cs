using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInventory : MonoBehaviour
{   
    public static UpgradeInventory Instance;

    private UpgradeManager upgradeManager;

    [System.Serializable]
    public class UpgradeEntry
    {
        public string upgradeName;
        public int count;
    }

    public List<UpgradeEntry> ownedUpgradesList = new List<UpgradeEntry>();

    // Run time use
    public Dictionary<string, int> ownedUpgrades = new Dictionary<string, int>();

    // Sync the dictionary with the list for Inspector visibility
    private void OnValidate()
    {
        ownedUpgrades.Clear();
        foreach (var entry in ownedUpgradesList)
        {
            if (!string.IsNullOrEmpty(entry.upgradeName))
            {
                ownedUpgrades[entry.upgradeName] = entry.count;
            }
        }
    }

    // Add an upgrade to the inventory
    public void AddUpgrade(string upgradeName)
    {
        if (ownedUpgrades.ContainsKey(upgradeName))
        {
            ownedUpgrades[upgradeName]++;
        }
        else
        {
            ownedUpgrades.Add(upgradeName, 1);
        }

        // Update the list for the Inspector
        SyncListWithDictionary();
    }

    public void RemoveUpgrade(string upgradeName, int count)
    {
        if (ownedUpgrades.ContainsKey(upgradeName))
        {
            ownedUpgrades[upgradeName] -= count;

            if (ownedUpgrades[upgradeName] <= 0)
            {
                ownedUpgrades.Remove(upgradeName);
            }
            SyncListWithDictionary();
            Debug.Log($"Removed {count} of upgrade '{upgradeName}'.");
        }
        else
        {
            Debug.LogWarning($"Upgrade '{upgradeName}' not found in inventory.");
        }
    
    }

    // Sync the list with the dictionary
    private void SyncListWithDictionary()
    {
        ownedUpgradesList.Clear();
        foreach (var pair in ownedUpgrades)
        {
            ownedUpgradesList.Add(new UpgradeEntry
            {
                upgradeName = pair.Key,
                count = pair.Value
            });
        }
    }
}