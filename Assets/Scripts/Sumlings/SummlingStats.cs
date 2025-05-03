using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SummlingAIState { Chase, Attack, Roam, Idle}
public abstract class SummlingStats : MonoBehaviour
{

    [Header("Damage")]
    public float damage;
    public float damageCooldown;
    public float critChance;
    public float critMultiplier;
    public float attackRange;
    private bool triggerDOT;

    [Header("Movement Stats")]
    public float movementSpeed;
    public float roamRadius;

    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float focusTime = 5f;
    public LayerMask enemyLayer;
    protected Transform currentTarget;
    protected Transform focusTarget;
    protected float focusTimer;
    protected float attackCooldownTimer;

    [Header("Roaming")]
    public float playerFollowThreshold = 3f; // Distance before updating roam position

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
}
