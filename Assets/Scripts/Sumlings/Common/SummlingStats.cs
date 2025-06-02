using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UpgradeData;


public enum Specie { Aquatic, Insect, Plushie}

public enum Mark { Newborn, Child, Adult}

public enum DOTType { None, Poison, Fire}
public enum Archetype { Melee, Range, Mix}
public enum SummlingAIState { Chase, Attack, Roam, Idle, ReturnToPlayer, PersonalObj}
public abstract class SummlingStats : MonoBehaviour
{
    [Header("Identifier")]
    public string summlingName;
    public Specie specie;
    public Mark mark;
    public string description;
    public Sprite icon;

    [Header("Archetype")]
    public Archetype archetype;

    // In SummlingStats.cs
    [Header("Mark Affected Stats")]
    [Tooltip("Which stats should be multiplied by the Mark bonus")]
    public List<StatType> affectedStatsByMark = new List<StatType>();


    [Tooltip("Bonus Modifiers")] public List<StatModifier> effects;

    [System.Serializable]
    public struct StatModifier
    {
        [Tooltip("What stat value will be affected? " +
        "Example: If you want Damage to increase use Damage")] public StatType statType;
        [Tooltip("Value of effect. Example: 10 if you want +10 Damage")] public float value;
        [Tooltip("Use decimal equivalents if Percentage.")] public bool isPercentage;
    }

    

    [Header("Damage Stats")]
    public float damage;
    public float damageCooldown;
    public float critChance;
    public float critMultiplier;
    

   

    [Header("Movement Stats")]
    public float movementSpeed;

    [Header("AI Radius")]
    public float attackRange = 1f;
    public float detectionRange = 8f;
    public float roamRadius = 10f;
    

    public float focusTime = 5f;
    public LayerMask enemyLayer;
    protected Transform currentTarget;
    protected Transform focusTarget;
    protected float focusTimer;
    protected float attackCooldownTimer;

    [Header("Player Leash Threshold")]
    public float leashRadius = 1f; 
    public float returnToPlayerSpeedMultiplier = 1.2f;

    [Header("References")]
    [HideInInspector]public GameObject player;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public float CalculateDamage()
    {
        bool isCrit = Random.Range(0f, 100f) <= critChance;
        return isCrit ? damage * critMultiplier : damage;
    }

     // Shows the radius of Summling stats for an easier tuning
    public void DrawGizmosContent()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, leashRadius);

        //// Draw target destination wire cube
        //Gizmos.color = Color.green;
        //Vector3 targetPosition = currentTarget != null ? currentTarget.position : roamPosition;
        //Gizmos.DrawWireCube(targetPosition, Vector3.one);
    }

    // In SummlingStats.cs
    public void ApplyMarkMultiplier(float multiplier)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            StatModifier mod = effects[i];
            if (affectedStatsByMark.Contains(mod.statType))
            {
                mod.value *= multiplier;

                // Update the struct in list
                effects[i] = mod; 
            }
        }
    }

    public float GetTransmuteValue()
    {
        return Mathf.Floor(Random.Range(1, 100f));
    }
}
