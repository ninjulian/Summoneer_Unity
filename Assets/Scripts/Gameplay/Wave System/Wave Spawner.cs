using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class WaveSpawner : MonoBehaviour
{
    [Header("Settings")]
    public float spawnDelay = 2f;
    public float spawnRadius = 15f;
    public LayerMask spawnCheckMask;

    [Header("References")]
    public Transform player;
    public GameObject[] enemyPrefabs;

    private WaveManager waveManager;
    [HideInInspector] public bool isSpawning = false;
    private Vector3 lastSpawnAttemptPosition;
    private bool lastAttemptValid;


    public bool usingSpawner = false;

    //private int enemiesSpawned = 0;

    private void OnEnable()
    {
        waveManager = GetComponent<WaveManager>();
        // Adds a listener for the onWaveStarted unity Event, when invoked will run Start Spawning
        waveManager.onWaveStarted.AddListener(StartSpawning);
    }

    private void StartSpawning()
    {
        if (!isSpawning && player != null && usingSpawner)
        {
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        int enemiesToSpawn = waveManager.GetTargetEnemies();
        int enemiesSpawned = 0;

        while (enemiesSpawned < enemiesToSpawn)
        {
            if (SpawnEnemy()) // Only increment if spawning succeeded
            {
                enemiesSpawned++;
            }
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    private bool SpawnEnemy()
    {
        if (player != null)
        {
            Vector3 spawnPosition;

            int attempts = 0;
            const int maxAttempts = 10; // Prevent infinite loops

            do
            {
                spawnPosition = GetValidSpawnPosition();
                attempts++;
                if (attempts >= maxAttempts && player != null) return false;
            }
            while (spawnPosition == Vector3.zero);

            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];


            GameObject enemy = Instantiate(randomEnemy, spawnPosition, Quaternion.identity);
            //enemy.SetActive(false);
            
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            //Scaling enemy Stats
            NewEnemyStats(enemyStats);
            //enemy.SetActive(true);

            waveManager.RegisterEnemy();
            enemy.GetComponent<EnemyStats>().onDeath.AddListener(waveManager.EnemyDefeated);
            return true;
        }
        else
        {
            return false;
        }
    }


    private Vector3 GetValidSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Record spawn attempt for debugging
        lastSpawnAttemptPosition = spawnPos;
        lastAttemptValid = !Physics.CheckSphere(spawnPos, 1f, spawnCheckMask);

        return lastAttemptValid ? spawnPos : Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (lastSpawnAttemptPosition != Vector3.zero)
        {
            Gizmos.color = lastAttemptValid ? Color.green : Color.red;
            Gizmos.DrawWireSphere(lastSpawnAttemptPosition, 1f);
        }
    }

    void NewEnemyStats(EnemyStats enemyStats)
    {
        //Hybrid scaling with the use of linear and exponential formula
        //Stat=(Base_Stat×Exp_Multiplier Wave)+(Wave×Linear_Bonus
        enemyStats.maxHealth = Mathf.Floor((enemyStats.maxHealth * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 5f));
        enemyStats.currentHealth = enemyStats.maxHealth;
        enemyStats.damage = Mathf.Floor((enemyStats.damage * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 0.5f));
        enemyStats.defense = Mathf.Floor((enemyStats.defense * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 0.2f));
        enemyStats.movementSpeed = Mathf.Floor((enemyStats.movementSpeed * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 0.1f));
    }
}