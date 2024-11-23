using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [Header("Pool Configuration")]
    public GameObject prefab; // Single prefab for this pool
    public int poolSize = 25; // Default pool size
    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    private void Awake()
    {
        // Pre-fill the pool
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private void CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }

    public GameObject GetObject()
    {
        if (poolQueue.Count == 0)
        {
            CreateNewObject();
        }

        GameObject obj = poolQueue.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
    public void ReturnAllObjects()
{
    foreach (var obj in poolQueue)
    {
        ReturnObject(obj);
    }
    poolQueue.Clear(); // Clear the list of active objects
}


    public List<GameObject> GetWave(int waveSize)
    {
        List<GameObject> wave = new List<GameObject>();
        for (int i = 0; i < waveSize; i++)
        {
            wave.Add(GetObject());
        }
        return wave;
    }
}
