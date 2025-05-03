using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider damageSlider;
    public Slider healthSlider;


    [Header("Stats")]
    [SerializeField] private StatClass entityStats;

    private Coroutine hpCoroutine;

    void Start()
    {
        entityStats = GetComponent<StatClass>();
        InitializeSliders();

    }


    public void InitializeSliders()
    {
        healthSlider.maxValue = entityStats.maxHealth;
        damageSlider.maxValue = entityStats.maxHealth;

        healthSlider.value = entityStats.maxHealth;
        damageSlider.value = entityStats.maxHealth;

    }


    public void StartHpUIUpdate(float damageValue)
    {
        if (hpCoroutine != null)
            StopCoroutine(hpCoroutine); // stop previous one if still running

        hpCoroutine = StartCoroutine(UpdateHpUICoroutine(damageValue));
    }

    private IEnumerator UpdateHpUICoroutine(float damageValue)
    {
        while (Mathf.Abs(healthSlider.value - entityStats.currentHealth) > 0.01f ||
               Mathf.Abs(damageSlider.value - healthSlider.value) > 0.01f)
        {
            healthSlider.value = Mathf.MoveTowards(healthSlider.value, entityStats.currentHealth, (3f * damageValue) * Time.deltaTime);
            damageSlider.value = Mathf.MoveTowards(damageSlider.value, healthSlider.value, ((damageValue / 2f) + damageValue) * Time.deltaTime);

            yield return null; // wait until next frame
        }

        // Final snap to precise value (optional cleanup)
        healthSlider.value = entityStats.currentHealth;
        damageSlider.value = entityStats.currentHealth;

        hpCoroutine = null; // finished
    }
}
