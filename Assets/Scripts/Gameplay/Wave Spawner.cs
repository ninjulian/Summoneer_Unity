using System.Collections;
using UnityEngine;

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
    private bool isSpawning = false;

    private void Awake()
    {
        waveManager = GetComponent<WaveManager>();
        waveManager.onWaveStarted.AddListener(StartSpawning);
    }

    private void StartSpawning()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        int enemiesToSpawn = waveManager.GetTargetEnemies();

        while (enemiesToSpawn > 0)
        {
            SpawnEnemy();
            enemiesToSpawn--;
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            GameObject enemy = Instantiate(
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                spawnPosition,
                Quaternion.identity
            );

            waveManager.RegisterEnemy();
            enemy.GetComponent<EnemyStats>().onDeath.AddListener(waveManager.EnemyDefeated);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (!Physics.CheckSphere(spawnPos, 1f, spawnCheckMask))
        {
            return spawnPos;
        }
        return Vector3.zero;
    }
}