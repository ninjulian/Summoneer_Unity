// UpgradeData.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatType { Health, Damage, MoveSpeed, JumpHeight, FireRate, Defense, Luck, Affinity,}
//Need to add the rest
public enum UpgradeCategory { Survival, Movement, Damage, Summling }
//Makes sure to have a variety
public enum Tier { Common, Uncommon, Epic, Legendary }
//Helps with balancing

[CreateAssetMenu(menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{

    [Tooltip("Upgrade Icon Image")] public Sprite icon;
    public string upgradeName;
    [Tooltip("Upgrade Category")] public UpgradeCategory category;
    [Tooltip("Tier of the Upgrade")] public Tier tier;
    [Tooltip("Base Upgrade cost")] public int baseCost;
    [Tooltip("Upgrade Modifiers")] public List<StatModifier> effects;
    [Tooltip("Descrition of the Upgrade effects")] public string descriptionText;

    [System.Serializable]public struct StatModifier
    {
        [Tooltip("What stat value will be affected? " +
        "Example: If you want Damage to increase use Damage")] public StatType statType;
        [Tooltip("Value of effect. Example: 10 if you want +10 Damage")] public float value;
        [Tooltip("Use decimal equivalents if Percentage.")] public bool isPercentage;
    }
}