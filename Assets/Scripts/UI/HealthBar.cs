using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthSlider;
    public Slider damageSlider;

    [Header("Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthLerpSpeed = 5f;
    [SerializeField] private float delayedLerpSpeed = 2f;

    [Header("Stats")]
    [SerializeField] private StatClass entityStats;

    void Start()
    {
        entityStats = GetComponent<StatClass>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeSliders()
    {
        healthSlider.maxValue = entityStats.maxHealth;
        damageSlider.maxValue = entityStats.maxHealth;

        healthSlider.value = entityStats.maxHealth;
        damageSlider.value = entityStats.maxHealth;

    }

    public void UpdateHpUI()
    {
        healthSlider.value = entityStats.currentHealth / entityStats.maxHealth;

        // Smoothly transition health values
        //    healthSlider.value = Mathf.MoveTowards(healthSlider.value, entityStats.currentHealth, healthLerpSpeed * Time.deltaTime);
        //    damageSlider.value = Mathf.MoveTowards(damageSlider.value, healthSlider.value, delayedLerpSpeed * Time.deltaTime);
    }


}
