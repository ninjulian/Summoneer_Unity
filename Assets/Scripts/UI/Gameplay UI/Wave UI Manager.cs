using TMPro;
using UnityEngine;

public class WaveUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;

    [Header("UI Elements")]
    public TMP_Text waveCounterText;
    public TMP_Text countdownText;
    public TMP_Text enemyCounter;

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
        waveCounterText.text = "Wave : " + waveManager.currentWave;
    }

    private void UpdateCountdown()
    {
        countdownText.text = waveManager.IsCountingDown ?
            $"Next wave in: {Mathf.CeilToInt(waveManager.CountdownTimer)}" :
            "";
    }

    private void UpdateEnemyCounter()
    {
        enemyCounter.text = $"Spawning: {waveManager.enemiesSpawned}/{waveManager.TargetEnemies}";
    }
}