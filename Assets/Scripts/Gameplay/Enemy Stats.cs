using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;

public class EnemyStats : StatClass
{
    [Header("Enemy Specific")]
    public float attackRange = 2f;

    //Damage text prefab
    public GameObject DamageText;
    public float spawnRadius = 1.0f;

    private DamageHandler damageHandler;
    private Outline outline;

    public UnityEvent onDeath;

    private void Awake()
    {
        base.Start();
        damageHandler = gameObject.GetComponent<DamageHandler>();
        damageHandler.Initialize(this);
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;

        PlayerShoot.OnEnemyFocused += HandleEnemyFocused;

    }

    private void OnDestroy()
    {
        
        onDeath?.Invoke();
        //Destroy(gameObject);
        // Unsubscribe to prevent memory leaks
        PlayerShoot.OnEnemyFocused -= HandleEnemyFocused;
    }

    private void HandleEnemyFocused(Transform focusedEnemy)
    {
        // Enable outline if this enemy is focused, disable otherwise
        outline.enabled = (focusedEnemy == transform);
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
        {    if (gameObject != null)
            {
                tmp.color = Color.white;
            }
        }
    }
}
