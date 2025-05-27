using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawner : MonoBehaviour
{
    public GameObject spawnableObject;
    public Transform spawnLocation;
    public int spawnLimit = 5;

    private List<GameObject> activeSpawns = new List<GameObject>();
    private int totalSpawnedCount = 0;

    private void FixedUpdate()
    {
        // Clean up destroyed objects from our list
        for (int i = activeSpawns.Count - 1; i >= 0; i--)
        {
            if (activeSpawns[i] == null)
            {
                activeSpawns.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool canSpawn = spawnLimit <= 0 || activeSpawns.Count < spawnLimit;

        if ((other.CompareTag("Player") || other.CompareTag("Player Projectile")) && canSpawn)
        {
            GameObject newObj = Instantiate(spawnableObject, spawnLocation.position, spawnLocation.rotation);
            activeSpawns.Add(newObj);
            totalSpawnedCount++;

    
        }
    }

}