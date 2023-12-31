using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Debug")]
    [Space(10)]
    public int playerCount = 2;

    [Header("Settings")]
    public int maxPlayers = 2;
    public int maxChunks = 10;
    public int maxBuildings = 20;
    public float buildingSpawnDistance = 100;
    public float validBuildingDistance = 10;
    public float chunkMeasurementSize = 5;
    public float activateInterval = 5;
    public float startPoolingDistance = 20;

    [Space(10f)]
    public Transform player;

    [Space(10f)]
    public GameObject start2;
    public GameObject start4;
    public GameObject start6;
    public GameObject start8;
    public GameObject road2;
    public GameObject road4;
    public GameObject road6;
    public GameObject road8;

    public List<GameObject> buildings = new List<GameObject>();

    private List<GameObject> roadChunks = new List<GameObject>();
    // Key: Prefab  Value: Size
    [ShowInInspector]
    private List<KeyValuePair<GameObject, float>> buidlingSizes = new List<KeyValuePair<GameObject, float>>();

    private GameObject startRoad;
    private float previousChunkPosition;
    private float previousBuildingPosition;

    private void Start()
    {
        SetupChunks();
        SetupBuilding();
    }

    private void SetupBuilding()
    {
        previousBuildingPosition = 0;

        for (int i = 0; i < maxBuildings; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, buildings.Count);
            GameObject building = buildings[randomIndex];
            MeshRenderer meshRenderer = building.GetComponentInChildren<MeshRenderer>();

            if (meshRenderer != null)
            {
                // Get the bounds of the GameObject's mesh
                Bounds bounds = meshRenderer.bounds;

                // Extract the size from the bounds
                Vector3 size = bounds.size;
                GameObject spawnedBuilding = Instantiate(building, new Vector3(-999, 0, 0), Quaternion.identity);
                spawnedBuilding.transform.SetParent(this.transform);
                spawnedBuilding.SetActive(true);
                buidlingSizes.Add(new KeyValuePair<GameObject, float>(spawnedBuilding, size.x));
            }
            else
            {
                Debug.LogError("Mesh Renderer component not found!");
            }
        }
    }

    private void Update()
    {
        CheckChunks();
        CheckBuildings();
    }

    private void CheckBuildings()
    {
        if (buidlingSizes.Count == 0) { return; }
        if (previousBuildingPosition < player.position.z + buildingSpawnDistance)
        {
            KeyValuePair<GameObject, float> building = GetRandomBuilding();
            float newBuildingPosition = previousBuildingPosition + building.Value + 1;
            building.Key.transform.position = OffsetOneValueBuilding(10, newBuildingPosition);
            previousBuildingPosition = newBuildingPosition;
        }
    }

    private void SetupChunks()
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }

        previousChunkPosition = 0;
        startRoad = Instantiate(GetStartRoadForPlayers(playerCount), OffsetOneValue(previousChunkPosition), Quaternion.identity);

        for (int i = 0; i < maxChunks; i++)
        {
            previousChunkPosition += chunkMeasurementSize;
            roadChunks.Add(Instantiate(GetRoadForPlayers(playerCount), OffsetOneValue(previousChunkPosition), Quaternion.identity));
        }
    }

    private void CheckChunks()
    {
        if (player.position.z > previousChunkPosition + chunkMeasurementSize + startPoolingDistance)
        {
            previousChunkPosition += chunkMeasurementSize;
            roadChunks[GetNextIndex()].transform.position = OffsetOneValue(previousChunkPosition);
        }
    }

    int nextIndex = -1;
    private int GetNextIndex()
    {
        nextIndex++;
        if (nextIndex >= roadChunks.Count)
        {
            nextIndex = 0;
        }

        return nextIndex;
    }

    private KeyValuePair<GameObject, float> GetRandomBuilding()
    {
        if (buidlingSizes.Count == 0)
        {
            Debug.LogError("No Buildings");
            return new KeyValuePair<GameObject, float>();
        }

        int randomIndex = UnityEngine.Random.Range(0, buidlingSizes.Count - 1);

        // The building is not in valid range of player
        if ((buidlingSizes[randomIndex].Key.transform.position.z - validBuildingDistance) > player.position.z)
            return GetRandomBuilding();

        return buidlingSizes[randomIndex];
    }

    private Vector3 OffsetOneValue(float offset)
    {
        return new Vector3(0, 0, offset);
    }

    private Vector3 OffsetOneValueBuilding(float leftOffset, float offset)
    {
        return new Vector3(-leftOffset, 0, offset);
    }

    private GameObject GetStartRoadForPlayers(int playerCount)
    {
        switch (maxPlayers)
        {
            case 2:
                return start2;

            case 4:
                return start4;

            case 6:
                return start6;

            case 8:
                return start8;

                default:
                return start2;
        }
    }

    private GameObject GetRoadForPlayers(int playerCount)
    {
        switch (maxPlayers)
        {
            case 2:
                return road2;

            case 4:
                return road4;

            case 6:
                return road6;

            case 8:
                return road8;

            default:
                return road2;
        }
    }
}
