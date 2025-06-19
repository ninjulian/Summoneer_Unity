using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WaveUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;

    [Header("UI Elements")]
    public TMP_Text waveCounterText;
    public TMP_Text countdownText;
    public TMP_Text enemyCounter;

    [Header("Animation Settings")]
    [SerializeField] private float pulseScaleAmount = 0.1f;
    [SerializeField] private float pulseDuration = 0.3f;

    // Store previous values to detect changes
    private string previousEnemyCounterText = "";
    private string previousWaveCounterText = "";

    private void OnValidate()
    {
        if (waveManager == null)
            waveManager = GetComponent<WaveManager>();
    }

    private void Update()
    {
        UpdateWaveCounter();
        UpdateCountdown();
        UpdateEnemyCounter();
    }

    private void UpdateWaveCounter()
    {
        string newText = "Wave : " + waveManager.currentWave;
        if (previousWaveCounterText != newText)
        {
            waveCounterText.text = newText;
            PulseAnimation(waveCounterText);
            previousWaveCounterText = newText;
        }
    }

    private void UpdateCountdown()
    {
        string newText = waveManager.IsCountingDown ?
            "Next wave in: " + Mathf.CeilToInt(waveManager.CountdownTimer).ToString() : "";
        countdownText.text = newText;
    }

    private void UpdateEnemyCounter()
    {
        string prefix = waveManager.enemiesSpawned >= waveManager.TargetEnemies ? "Alive: " : "Spawning: ";
        string newText = prefix + waveManager.enemiesAlive + "/" + waveManager.TargetEnemies;

        if (previousEnemyCounterText != newText)
        {
            enemyCounter.text = newText;
            PulseAnimation(enemyCounter);
            previousEnemyCounterText = newText;
        }
    }

    private void PulseAnimation(TMP_Text textElement)
    {
        // Kill any existing animations on this object
        DOTween.Kill(textElement.transform);

        // Create pulse animation
        textElement.transform.localScale = Vector3.one;
        textElement.transform.DOPunchScale(Vector3.one * pulseScaleAmount, pulseDuration, 1, 0.5f)
            .SetEase(Ease.OutQuad);
    }
}