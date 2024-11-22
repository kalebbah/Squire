using System.Collections;
using System.Collections.Generic;
using UnityEditor.TestTools.CodeCoverage;
using UnityEngine;
using UnityEngine.UI; // Required for Image

public class EnemyManager : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerTag = "Player";
    private Transform player;

    [Header("Enemy Pools")]
    public List<ObjectPool> basicEnemyPools; // Pools for basic enemies
    public List<ObjectPool> advancedEnemyPools; // Pools for advanced enemies
    public ObjectPool bossPool; // Pool for the boss
    public GameObject bossHealthBarUI;

    [Header("Spawn Settings")]
    public float spawnRadius = 10f;
    public int initialEnemyCount = 10;
    public float waveInterval = 5f;
    public int maxSpawnCount = 100;

    private int currentEnemyCount = 0;
    private bool bossSpawned = false;
    private Coroutine spawnCoroutine;

    private DataPersistenceManager dataManager;
    [Header("Progress Meter")]
    public Image progressMeter; // Reference to the UI Image for the progress meter

    private void Start()
    {
        if (progressMeter != null)
        {
            progressMeter.fillAmount = 0f; // Initialize progress meter
        }
    }

    public void Configure(
        List<ObjectPool> basicPools,
        List<ObjectPool> advancedPools,
        ObjectPool bossPool,
        float spawnRadius,
        int initialEnemyCount,
        float waveInterval,
        int maxSpawnCount)
    {
        this.basicEnemyPools = basicPools;
        this.advancedEnemyPools = advancedPools;
        this.bossPool = bossPool;
        this.spawnRadius = spawnRadius;
        this.initialEnemyCount = initialEnemyCount;
        this.waveInterval = waveInterval;
        this.maxSpawnCount = maxSpawnCount;

        Initialize();
    }

    private void Initialize()
    {
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found!");
            return;
        }
        dataManager = FindObjectOfType<DataPersistenceManager>();

        StartSpawning();
    }

    private void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnWaves());
        }
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            // Wait until the game state is in 'Game'
            yield return new WaitUntil(() => GameManager.Instance.currentState == GameState.GAME);

            if (currentEnemyCount >= maxSpawnCount && !bossSpawned)
            {
                SpawnBoss();
                yield break; // Stop spawning waves after the boss appears
            }

            if (currentEnemyCount < maxSpawnCount / 2)
            {
                SpawnWave(basicEnemyPools, initialEnemyCount);
            }
            else
            {
                SpawnWave(advancedEnemyPools, initialEnemyCount);
            }

            UpdateProgressMeter();
            yield return new WaitForSeconds(waveInterval);
        }
    }

    private void SpawnWave(List<ObjectPool> pools, int waveSize)
    {
        if (pools.Count == 0)
        {
            Debug.LogWarning("No enemy pools available for spawning.");
            return;
        }

        StartCoroutine(SpawnWaveWithDelay(pools, waveSize, 0.5f)); // Add 0.5-second delay between spawns
    }

    private IEnumerator SpawnWaveWithDelay(List<ObjectPool> pools, int waveSize, float delay)
    {
        for (int i = 0; i < waveSize; i++)
        {
            // Generate a random angle for positioning around the player
            float angle = Random.Range(0f, 360f);
            float radians = angle * Mathf.Deg2Rad;

            // Calculate spawn position on the circle
            Vector3 spawnPosition = player.position + new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * spawnRadius;

            // Get an enemy from the selected pool
            ObjectPool selectedPool = pools[Random.Range(0, pools.Count)];
            GameObject enemy = selectedPool.GetObject();

            if (enemy != null)
            {
                enemy.transform.position = spawnPosition;
                enemy.transform.rotation = Quaternion.LookRotation(player.position - spawnPosition); // Face the player

                currentEnemyCount++;
                UpdateProgressMeter();
            }

            // Wait for the specified delay before spawning the next enemy
            yield return new WaitForSeconds(delay);
        }
    }

    private void UpdateProgressMeter()
    {
        if (progressMeter != null)
        {
            progressMeter.fillAmount = (float)currentEnemyCount / maxSpawnCount;
            dataManager.SetLevelProgression(progressMeter.fillAmount);
        }
    }

    private void SpawnBoss()
    {
        if (bossPool == null)
        {
            Debug.LogError("Boss pool not assigned.");
            return;
        }

        bossSpawned = true;

        GameObject boss = bossPool.GetObject();
        Vector3 spawnPosition = player.position + new Vector3(0, 0, spawnRadius);
        boss.transform.position = spawnPosition;
        boss.transform.rotation = Quaternion.identity;

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.SetActive(true);
        }

        BossHandle bossComponent = boss.GetComponent<BossHandle>();
        if (bossComponent != null)
        {
            EventManager<HealthHandle>.TriggerEvent(EventKey.BOSS_HEALTH_UPDATED, bossComponent.GetHealthHandle());

            bossComponent.GetHealthHandle().OnDeath += () =>
            {
                bossHealthBarUI.SetActive(false);
                Debug.Log("Boss defeated!");
                bossPool.ReturnObject(boss);
                Restart();
            };
        }
    }

    public void Restart()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        currentEnemyCount = 0;
        bossSpawned = false;

        if (progressMeter != null)
        {
            progressMeter.fillAmount = 0f; // Reset progress meter
        }

        StartSpawning();
    }
}
