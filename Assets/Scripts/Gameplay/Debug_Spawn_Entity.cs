using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Debug_Spawn_Entity : MonoBehaviour
{
    public List<GameObject> entityPrefabs;
    public List<GameObject> spawnedPrefabs = new List<GameObject>();



    // Spawn Logic
    public void SpawnEntity(string entityName, Transform playerLocation)
    {
        foreach (GameObject prefab in entityPrefabs)
        {
            if (prefab.name == entityName)
            {
                GameObject spawned = Instantiate(prefab, playerLocation.position, Quaternion.identity);
                spawnedPrefabs.Add(spawned);
                Debug.Log($"Spawned entity: {entityName}");
                return;
            }
        }
        Debug.LogWarning($"Entity with name {entityName} not found in prefabs list.");
    }

    // Clearing Logic

    public void DespawnEntity(string entityName, int count)
    {
        foreach (GameObject prefab in spawnedPrefabs)
        {   
            if (prefab.name == entityName)
            {
                for (int i = 0; i > count; i++)
                {
                    Destroy(prefab);
                }
            }

        }

    }

}
