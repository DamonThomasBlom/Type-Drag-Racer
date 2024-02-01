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
    public int maxRoadChunks = 10;
    public int maxBuildings = 20;
    public float buildingSpawnDistance = 100;
    public float validBuildingDistance = 10;
    public float chunkMeasurementSize = 5;
    public float activateInterval = 5;
    public float startPoolingDistance = 20;

    [Space(10f)]
    public Transform player;

    [Space(10f)]
    public PrefabReferences prefabReferences;

    private List<GameObject> roadChunks = new List<GameObject>();
    // Key: Prefab  Value: Size
    [ShowInInspector]
    private List<KeyValuePair<GameObject, float>> buidlingSizes = new List<KeyValuePair<GameObject, float>>();

    private GameObject startRoad;
    private float previousChunkPosition;
    private float previousBuildingPosition;

    private void Start()
    {
        SetupRoadChunks();
        SetupBuilding();
    }

    private void SetupBuilding()
    {
        previousBuildingPosition = 0;

        for (int i = 0; i < maxBuildings; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, prefabReferences.buildings.Count);
            GameObject building = prefabReferences.buildings[randomIndex];
            MeshRenderer meshRenderer = building.GetComponentInChildren<MeshRenderer>();

            // Get all the Mesh Renderers attached to the object
            MeshRenderer[] meshRenderers = building.GetComponentsInChildren<MeshRenderer>();

            // Create a new Bounds object to store the combined bounds
            Bounds combinedBounds = new Bounds();

            // Iterate through each Mesh Renderer and expand the combined bounds
            foreach (MeshRenderer renderer in meshRenderers)
            {
                if (renderer.bounds.size != Vector3.zero)
                {
                    if (combinedBounds.size == Vector3.zero)
                        combinedBounds = renderer.bounds;
                    else
                        combinedBounds.Encapsulate(renderer.bounds);
                }
            }

            // Extract the size from the bounds
            Vector3 size = combinedBounds.size;
            GameObject spawnedBuilding = Instantiate(building, new Vector3(-999, 0, -999), Quaternion.identity);
            spawnedBuilding.transform.SetParent(this.transform);
            spawnedBuilding.SetActive(true);
            buidlingSizes.Add(new KeyValuePair<GameObject, float>(spawnedBuilding, size.x));
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
            float newBuildingPosition = previousBuildingPosition + building.Value;
            building.Key.transform.position = OffsetOneValueBuilding(10, newBuildingPosition);
            previousBuildingPosition = newBuildingPosition;
        }
    }

    private void SetupRoadChunks()
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }

        previousChunkPosition = 0;
        startRoad = Instantiate(prefabReferences.GetStartRoadPrefab(playerCount), OffsetZValue(previousChunkPosition), Quaternion.identity);

        for (int i = 0; i < maxRoadChunks; i++)
        {
            previousChunkPosition += chunkMeasurementSize;
            GameObject roadChunk = Instantiate(prefabReferences.GetRoadPrefab(playerCount), OffsetZValue(previousChunkPosition), Quaternion.identity);
            roadChunk.transform.parent = this.transform;
            roadChunks.Add(roadChunk);
        }
    }

    private void CheckChunks()
    {
        if (player.position.z > previousChunkPosition + chunkMeasurementSize + startPoolingDistance)
        {
            previousChunkPosition += chunkMeasurementSize;
            roadChunks[GetNextIndex()].transform.position = OffsetZValue(previousChunkPosition);
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
        if ((buidlingSizes[randomIndex].Key.transform.position.z + validBuildingDistance) > player.position.z)
            return GetRandomBuilding();

        return buidlingSizes[randomIndex];
    }

    private Vector3 OffsetZValue(float offset)
    {
        return new Vector3(0, 0, offset);
    }

    private Vector3 OffsetOneValueBuilding(float leftOffset, float offset)
    {
        return new Vector3(-leftOffset, 0, offset + 0.5f);
    }
}
