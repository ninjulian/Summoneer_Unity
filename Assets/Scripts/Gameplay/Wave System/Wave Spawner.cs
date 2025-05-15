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

    //private int enemiesSpawned = 0;

    private void Awake()
    {
        waveManager = GetComponent<WaveManager>();
        // Adds a listener for the onWaveStarted unity Event, when invoked will run Start Spawning
        waveManager.onWaveStarted.AddListener(StartSpawning);
    }

    private void StartSpawning()
    {
        if (!isSpawning && player != null)
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

            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPosition, Quaternion.identity);
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
}