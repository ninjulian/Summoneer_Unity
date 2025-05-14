// UpgradeData.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
///  MaxHealth, Health, Damage, MovementSpeed, JumpHeight, FireRate, Defense, Luck, Affinity, 
///  SummlingDamage, SummlingRange, SummlingCC, SummlingCM, SummlingMovementSpeed
/// </summary>
public enum StatType { MaxHealth, Health, Damage, MovementSpeed, JumpHeight, FireRate, Defense, Luck, Affinity, 
    SummlingDamage, SummlingRange, SummlingCC, SummlingCM, SummlingMovementSpeed}
//Need to add projectile variants
//Makes sure to have a variety
public enum UpgradeCategory { Survival, Movement, Damage, QOL, Summling }

public enum Tier { Common, Uncommon, Epic, Legendary }
//Helps with balancing

[CreateAssetMenu(menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{

    [Tooltip("Upgrade Icon Image")] public Sprite icon;
    public string upgradeName;
    [Tooltip("Upgrade Category")] public UpgradeCategory category;
    [Tooltip("Stack limit count. 0 means no limit")] public int stackLimit = 0;
    [HideInInspector]   public int currentStackCount;
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