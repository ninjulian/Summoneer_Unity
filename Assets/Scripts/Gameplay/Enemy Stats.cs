using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class EnemyStats : StatClass
{
    [Header("Enemy Specific")]
    public float attackRange = 2f;

    //Damage text prefab
    public GameObject DamageText;
    public float spawnRadius = 1.0f;

    private DamageHandler damageHandler;

    private void Awake()
    {
        base.Start();
        damageHandler = gameObject.GetComponent<DamageHandler>();
        damageHandler.Initialize(this);
    }

    public override void TakeDamage(float incomingDamage, DamageHandler.DOTType? dotType = null)
    {   
        currentHealth -= incomingDamage;

        if (DamageText)
        {
            ShowDamageText(incomingDamage, dotType);
        }
    }

    void ShowDamageText(float damageValue, DamageHandler.DOTType? dotType)
    {

        Vector3 randomOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(0.5f, 1.5f), Random.Range(-spawnRadius, spawnRadius));
        Vector3 spawnPosition = transform.position + randomOffset;

        var go = Instantiate(DamageText, spawnPosition, Quaternion.identity);

        TextMeshProUGUI tmp = go.GetComponentInChildren<TextMeshProUGUI>();

        tmp.text = damageValue.ToString();

        var currentDOT = damageHandler.GetCurrentDOTType();

        //Changes text colour dependent on the DOTType
        if (dotType.HasValue)
        {
            switch (dotType.Value)
            {
                case DamageHandler.DOTType.Fire:
                    tmp.color = Color.red;
                    break;
                case DamageHandler.DOTType.Poison:
                    tmp.color = new Color(0.5f, 0f, 1f);
                    break;
            }
        }
        else
        {
            tmp.color = Color.white;
        }
    }
}
