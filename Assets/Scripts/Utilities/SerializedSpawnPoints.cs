using JetBrains.Annotations;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SerializedSpawnPoints : SerializedMonoBehaviour
{
    public static SerializedSpawnPoints Instance;

    private void Awake()
    {
        Instance = this;
    }

    [ShowInInspector]
    public Dictionary<Transform, bool> SpawnPoints = new Dictionary<Transform, bool>();

    [Button]
    public void AssignSpawnPoints()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            SpawnPoints.Add(child, false);
        }
    }

    public Transform GetRandomPlayerSpawnPoint()
    {
        if (SpawnPoints.Count == 0)
        {
            Debug.LogError("No player spawn points available.");
            return null;
        }

        List<Transform> availableSpawnPoints = new List<Transform>();

        foreach (var spawnPoint in SpawnPoints)
        {
            if (!spawnPoint.Value)
            {
                availableSpawnPoints.Add(spawnPoint.Key);
            }
        }

        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogWarning("All player spawn points are occupied.");
            return null;
        }

        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        Transform randomSpawnPoint = availableSpawnPoints[randomIndex];

        Debug.Log("Get Spawn Point ran at " + Time.time + " with index " +  randomIndex);

        // Mark the selected spawn point as occupied
        SpawnPoints[randomSpawnPoint] = true;

        return randomSpawnPoint;
    }

    public string GetTakenSpawnPoints()
    {
        List<bool> result = new List<bool>();

        foreach(var spawnPoint in SpawnPoints)
        {
            result.Add(spawnPoint.Value);
        }

        return JsonConvert.SerializeObject(result);
    }

    public void UpdateTakenSpawnPoints(string data)
    {
        List<bool> TakenSpawnPoints = JsonConvert.DeserializeObject<List<bool>>(data);

        for(int i = 0; i < SpawnPoints.Count; i++)
        {
            SpawnPoints[SpawnPoints.ElementAt(i).Key] = TakenSpawnPoints[i];
        }
    }

    [Button]
    void MarkSpawnpointsAsNotOccupied()
    {
        // Create a separate list to store spawn points that need bots
        List<Transform> unoccupiedSpawnPoints = new List<Transform>();

        // Iterate over the spawn points to find unoccupied ones
        foreach (var spawnPoint in SpawnPoints)
        {
            unoccupiedSpawnPoints.Add(spawnPoint.Key);
        }

        foreach (var spawnPoint in unoccupiedSpawnPoints)
        {
            SpawnPoints[spawnPoint] = false;
        }
    }
}
