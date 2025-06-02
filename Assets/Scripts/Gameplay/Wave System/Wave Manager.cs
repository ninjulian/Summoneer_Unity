using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("Settings")]
    public float waveFactor = 1.5f;
    public float levelFactor = 2.0f;
    public float StatIncreaseFactor = 1.2f;

    [Header("Debug")]
    public int currentWave = 0;
    public int enemiesAlive = 0;
    [SerializeField] private int targetEnemies = 0;

    [Header("Events")]
    [HideInInspector]public UnityEvent onWaveStarted;
    [HideInInspector]public UnityEvent onWaveCompleted;

    [Header("Wave Timing")]
    public float timeBetweenWaves = 5f;

    public int CurrentWave => currentWave;
    public int EnemiesAlive => enemiesAlive;
    public int TargetEnemies => targetEnemies;
    public float CountdownTimer { get; private set; }
    public bool IsCountingDown { get; private set; }


    private PlayerStats playerStats;
    private WaveSpawner waveSpawner;

    public UIManager uiManager;
    private UpgradeManager upgradeManager;

    [HideInInspector]public int enemiesSpawned = 0;

    // Highest Wave count tracker
    private const string HIGHEST_WAVE_KEY = "HighestWave"; 

    private void OnEnable()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        waveSpawner = GetComponent<WaveSpawner>();
        upgradeManager = GetComponent<UpgradeManager>();
        StartNextWave();
    }

    public void Update()
    {
        
        if (IsCountingDown) 
        {
            // If the Manager is counting down update the countdown timer
            CountdownTimer -= Time.deltaTime;


            if (CountdownTimer <= 0f)
            {   
                // When count down reaches 0 start next wave
                IsCountingDown = false;
                StartNextWave();
            }
        }
    }

    public void StartNextWave()
    {   

        // +1 wave count
        currentWave++;

        // Overwrite if new highest Wave
        if (currentWave > PlayerPrefs.GetInt(HIGHEST_WAVE_KEY, 0))
        {
            PlayerPrefs.SetInt(HIGHEST_WAVE_KEY, currentWave);

            // Immediately save
            PlayerPrefs.Save(); 
        }

        //Calculate Enemy count to Spawn
        CalculateWave();

        // Listener will call function,
        // in this case Wave Spawner script will run StartSpawning
        onWaveStarted?.Invoke(); 
    }

    private void CalculateWave()
    {
        //Calculate Enemy count to Spawn
        targetEnemies = Mathf.RoundToInt((currentWave * waveFactor) + (playerStats.playerLevel * levelFactor));
    }


    public void RegisterEnemy()
    {
        enemiesAlive++;
        enemiesSpawned++;

    }

    public void EnemyDefeated()
    {   
        enemiesAlive--;

        //If enemies alive is 0 and all of enemies are spawned
        if (enemiesAlive <= 0 && enemiesSpawned >= targetEnemies)
        {
            // Show upgrade UI instead of completing wave immediately
            StartCoroutine(ShowUpgradeUICoroutine());
        }
    }

    public IEnumerator ShowUpgradeUICoroutine()
    {   
        // Start timer
        float remainingTime = uiManager.openTimer;

        // Shows the timer
        uiManager.openSystemTimer.SetActive(true);
        //yield return new WaitForSeconds(uiManager.openTimer);

        // Update countdown every frame
        while (remainingTime > 0)
        {   
            //Updates UI text for countdown
            uiManager.openSystemTimerText.text = Mathf.CeilToInt(remainingTime).ToString();
            remainingTime -= Time.deltaTime;
            yield return null; 
        }

        // Hide timer
        uiManager.openSystemTimer.SetActive(false);

        // Generate upgrades and reset UI
        UpgradeManager.Instance.GenerateUpgrades(CurrentWave);
        upgradeManager.hasRerolled = false;

        // Toggles upgrade UI
        uiManager.ToggleUpgradeUI();
    }

    public void CompleteWave()
    {

        // Starts the next wave
        StartCountdown();

        // Invoke other system
        onWaveCompleted?.Invoke(); 
    }

    // Call start countdown when done using the UpgradeUI
    public void StartCountdown()
    {
        if (enemiesAlive <= 0 && enemiesSpawned >= targetEnemies && !IsCountingDown)
        {   
            // Resets reroll boolean
            upgradeManager.hasRerolled = false;

            // Reset enemy trackers
            enemiesSpawned = 0;
            enemiesAlive = 0;

            // Start countdown
            IsCountingDown = true;
            CountdownTimer = timeBetweenWaves;
        }
    }

    // For difficulty scaling;
    public int GetTargetEnemies() => targetEnemies;
}