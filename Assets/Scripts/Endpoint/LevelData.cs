using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public string levelName; // Name of the level
    public string objectiveDescription; // Description of the level objective
    public float spawnRadius; // Radius for enemy spawning
    public int initialObjectCount; // Initial number of enemies
    public float initialSpawnInterval; // Initial spawn interval for enemies
    public int maxSpawnCount;

    public List<GameObject> basicEnemyPrefabs; // Prefabs for basic enemies
    public List<GameObject> advancedEnemyPrefabs; // Prefabs for advanced enemies
    public GameObject bossPrefab; // Prefab for the boss
}
