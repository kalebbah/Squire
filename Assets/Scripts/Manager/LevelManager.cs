using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    public List<LevelData> levels;
    private EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        int lvl = FindObjectOfType<DataPersistenceManager>().Level;
        Debug.Log(lvl);
        LoadLevel(lvl); // Start at level 0
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Count)
        {
            Debug.LogError("Invalid level index.");
            return;
        }
        LevelData currentLevel = levels[index];

        List<ObjectPool> basicPools = CreatePools(currentLevel.basicEnemyPrefabs);
        List<ObjectPool> advancedPools = CreatePools(currentLevel.advancedEnemyPrefabs);
        ObjectPool bossPool = CreatePool(currentLevel.bossPrefab);

        enemyManager.Configure(basicPools, advancedPools, bossPool, currentLevel.spawnRadius, currentLevel.initialObjectCount, currentLevel.initialSpawnInterval, currentLevel.maxSpawnCount);
    }

    private List<ObjectPool> CreatePools(List<GameObject> prefabs)
    {
        List<ObjectPool> pools = new List<ObjectPool>();
        foreach (var prefab in prefabs)
        {
            pools.Add(CreatePool(prefab));
        }
        return pools;
    }

    private ObjectPool CreatePool(GameObject prefab)
    {
        GameObject poolObject = new GameObject(prefab.name + "_Pool");
        ObjectPool pool = poolObject.AddComponent<ObjectPool>();
        pool.prefab = prefab;
        pool.poolSize = 25; // Default size
        return pool;
    }
}
