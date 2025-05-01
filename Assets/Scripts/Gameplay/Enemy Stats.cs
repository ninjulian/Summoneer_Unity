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

    public override void TakeDamage(float incomingDamage)
    {   
        currentHealth -= incomingDamage;

        if (DamageText)
        {
            ShowDamageText(incomingDamage);
        }
    }

    void ShowDamageText(float damageValue)
    {

        Vector3 randomOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(0.5f, 1.5f), Random.Range(-spawnRadius, spawnRadius));
        Vector3 spawnPosition = transform.position + randomOffset;

        var go = Instantiate(DamageText, spawnPosition, Quaternion.identity);

        TextMeshProUGUI tmp = go.GetComponentInChildren<TextMeshProUGUI>();

        tmp.text = damageValue.ToString();

        var currentDOT = damageHandler.GetCurrentDOTType();

       

        ////Changes text colour dependent on the DOTType
        //if (currentDOT.HasValue)
        //{
        //    switch (currentDOT.Value)
        //    {
        //        case DamageHandler.DOTType.Fire:
        //            tmp.color = Color.red;
        //            break;
        //        case DamageHandler.DOTType.Poison:
        //            tmp.color = Color.green;
        //            break;
        //    }
        //}
        //else
        //{
        //   tmp.color = Color.white;      
        //}

    }
}
