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
        // Find all matching entities in the spawnedPrefabs list
        List<GameObject> entitiesToRemove = new List<GameObject>();

        foreach (GameObject spawned in spawnedPrefabs)
        {
            if (spawned != null && spawned.name.StartsWith(entityName))
            {
                entitiesToRemove.Add(spawned);
            }
        }

        // Limit to the requested count
        int removeCount = Mathf.Min(count, entitiesToRemove.Count);

        // Remove the specified number of entities
        for (int i = 0; i < removeCount; i++)
        {
            if (entitiesToRemove[i] != null)
            {
                Destroy(entitiesToRemove[i]);
                spawnedPrefabs.Remove(entitiesToRemove[i]);
            }
        }

        Debug.Log($"Despawned {removeCount} of {entityName}");
    }

    public void ClearEntities()
    {
        foreach (GameObject prefab in spawnedPrefabs)
        {
            Destroy(prefab);
        }
    }

}
