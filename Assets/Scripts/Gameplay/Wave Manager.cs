using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("Settings")]
    public float waveFactor = 1.5f;
    public float levelFactor = 2.0f;

    [Header("Debug")]
    public int currentWave = 0;
    public int enemiesAlive = 0;
    [SerializeField] private int targetEnemies = 0;

    [Header("Events")]
    public UnityEvent onWaveStarted;
    public UnityEvent onWaveCompleted;

    [Header("Wave Timing")]
    public float timeBetweenWaves = 5f;

    public int CurrentWave => currentWave;
    public int EnemiesAlive => enemiesAlive;
    public int TargetEnemies => targetEnemies;
    public float CountdownTimer { get; private set; }
    public bool IsCountingDown { get; private set; }


    private PlayerStats level;

    private void Awake()
    {
        level = FindObjectOfType<PlayerStats>();
        StartNextWave();
    }

    public void Update()
    {
        if (IsCountingDown)
        {
            CountdownTimer -= Time.deltaTime;

            if (CountdownTimer <= 0f)
            {
                IsCountingDown = false;
                StartNextWave();
            }
        }
    }

    public void StartNextWave()
    {
        currentWave++;
        CalculateWave();
        onWaveStarted?.Invoke();
    }

    private void CalculateWave()
    {
        targetEnemies = Mathf.RoundToInt(
            (currentWave * waveFactor) +
            (level.playerLevel * levelFactor)
        );
    }



    public void RegisterEnemy()
    {
        enemiesAlive++;
    }

    public void EnemyDefeated()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && targetEnemies <= 0)
        {
            CompleteWave();
        }
    }

    private void CompleteWave()
    {   
        StartCountdown();
        onWaveCompleted?.Invoke();
    }

    // Add countdown methods
    private void StartCountdown()
    {
        IsCountingDown = true;
        CountdownTimer = timeBetweenWaves;
    }


    public int GetTargetEnemies() => targetEnemies;
}