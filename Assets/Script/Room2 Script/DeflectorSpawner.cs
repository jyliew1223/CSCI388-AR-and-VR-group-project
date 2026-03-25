using UnityEngine;
using System.Collections.Generic;

public class DeflectorSpawner : MonoBehaviour
{
    public GameObject deflectorPrefab;     // Prefab to spawn
    public Transform spawnPoint;           // Spawn location
    public int maxDeflectors = 5;          // Max allowed at once

    private Queue<GameObject> spawnedDeflectors = new Queue<GameObject>();

    public void SpawnDeflector()
    {
        if (deflectorPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("Deflector Prefab or Spawn Point is not assigned.");
            return;
        }

        // If over limit, destroy the oldest one
        if (spawnedDeflectors.Count >= maxDeflectors)
        {
            GameObject oldest = spawnedDeflectors.Dequeue();
            Destroy(oldest);
        }

        GameObject newDeflector = Instantiate(deflectorPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedDeflectors.Enqueue(newDeflector);
    }
}
