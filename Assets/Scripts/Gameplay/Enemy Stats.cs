using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : StatClass
{
    [Header("Enemy Specific")]
    public float attackRange = 2f;

    private DamageHandler damageHandler;

    private void Awake()
    {
        base.Start();
        damageHandler = gameObject.AddComponent<DamageHandler>();
        damageHandler.Initialize(this);
    }

    public override void TakeDamage(float incomingDamage)
    {
        damageHandler.ReceiveDamage(incomingDamage);
    }
}
