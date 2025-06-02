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

        // Get number count of max enemies to spawn
        int enemiesToSpawn = waveManager.GetTargetEnemies();
        int enemiesSpawned = 0;

        while (enemiesSpawned < enemiesToSpawn)
        {
            // Only add increments if spawned
            if (SpawnEnemy())
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
            const int maxAttempts = 10; 

            // Try to find valid spawning locations
            do
            {
                spawnPosition = GetValidSpawnPosition();
                attempts++;
                if (attempts >= maxAttempts && player != null) return false;
            }

            // Try again if invalid
            while (spawnPosition == Vector3.zero);

            //Pick random enemy
            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];


            GameObject enemy = Instantiate(randomEnemy, spawnPosition, Quaternion.identity);
            //enemy.SetActive(false);

            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

            //Scaling enemy Stats
            NewEnemyStats(enemyStats);
            //enemy.SetActive(true);

            // Register Enemy to waveManager
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
        // Finds a random point inside circle aorund player
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Checks if area is valid
        lastSpawnAttemptPosition = spawnPos;
        lastAttemptValid = !Physics.CheckSphere(spawnPos, 1f, spawnCheckMask);

        return lastAttemptValid ? spawnPos : Vector3.zero;
    }

    private void OnDrawGizmos()
    {   //Shows where spawning area is
        if (lastSpawnAttemptPosition != Vector3.zero)
        {
            Gizmos.color = lastAttemptValid ? Color.green : Color.red;
            Gizmos.DrawWireSphere(lastSpawnAttemptPosition, 1f);
        }
    }

    // Applies new Enemy stats detemiend by the wave
    void NewEnemyStats(EnemyStats enemyStats)
    {
        //Hybrid scaling with the use of linear and exponential formula
        enemyStats.maxHealth = Mathf.Floor((enemyStats.maxHealth * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 2f));
        enemyStats.currentHealth = enemyStats.maxHealth;
        enemyStats.damage = Mathf.Floor((enemyStats.damage * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 0.1f));
        enemyStats.defense = Mathf.Floor((enemyStats.defense * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 0.1f));
        enemyStats.movementSpeed = Mathf.Floor((enemyStats.movementSpeed * waveManager.StatIncreaseFactor) + (waveManager.currentWave * 0.1f));
    }
}