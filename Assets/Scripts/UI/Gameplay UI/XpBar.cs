using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    [SerializeField] public Slider xpSlider;
    [SerializeField] private float fillSpeed = 0.5f; 

    private PlayerStats playerStats;
    private Coroutine currentAnimation;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        xpSlider.minValue = 0;
    }

    public void UpdateXPBar()
    {
        xpSlider.maxValue = playerStats.xpRequired;

        // Stop any existing animation
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        // Start new animation
        currentAnimation = StartCoroutine(AnimateXPBar(playerStats.currentXP));
    }

    private IEnumerator AnimateXPBar(float targetXP)
    {
        float startValue = xpSlider.value;
        float duration = Mathf.Abs(targetXP - startValue) / fillSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            xpSlider.value = Mathf.Lerp(startValue, targetXP, t);

            // Update max value
            xpSlider.maxValue = playerStats.xpRequired;

            // Clamp value to prevent overshooting
            if (xpSlider.value > xpSlider.maxValue)
            {
                xpSlider.value = xpSlider.maxValue;
                break;
            }

            yield return null;
        }

        xpSlider.value = targetXP;
        currentAnimation = null;
    }
}