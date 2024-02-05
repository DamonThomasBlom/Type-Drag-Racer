using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Debug")]
    [Space(10)]
    public int playerCount = 2;

    [Header("Settings")]
    [Tooltip("Max pooled roads")]
    public int maxRoadChunks = 10;
    [Tooltip("Max pooled buildings")]
    public int maxBuildings = 20;
    [Tooltip("Building spawn distance from player")]
    public float buildingSpawnDistance = 100;
    [Tooltip("Building despawn distance")]
    public float buldingDespawnDistance = 10;
    [Tooltip("Road offset distance")]
    public float roadMeasurementSize = 5;
    [Tooltip("When should we start pooling")]
    public float startPoolingRoadDistance = 20;

    [Space(10f)]
    public Transform player;

    [Space(10f)]
    public PrefabReferences prefabReferences;

    private List<GameObject> roadChunks = new List<GameObject>();
    // Key: Prefab  Value: Size
    [ShowInInspector]
    private List<KeyValuePair<GameObject, float>> buidlingSizes = new List<KeyValuePair<GameObject, float>>();

    private float previousChunkPosition;
    private float previousBuildingPosition;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (player == null)
        {
            if (Player.Instance.playerTransform == null)
            {
                Invoke(nameof(Init), 1);
                return;
            }
            player = Player.Instance.playerTransform;
        }

        SetupRoadChunks();
        SetupBuilding();
    }

    private void Update()
    {
        if (player == null) { return; }
        CheckChunks();
        CheckBuildings();
    }

    private void SetupBuilding()
    {
        // Start buildings from behind the player
        previousBuildingPosition = -100;

        for (int i = 0; i < maxBuildings; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, prefabReferences.buildings.Count);
            GameObject building = prefabReferences.buildings[randomIndex];

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


    int buildingLeftOffset = 7;

    private void CheckBuildings()
    {
        if (buidlingSizes.Count == 0) { return; }
        if (previousBuildingPosition < player.position.z + buildingSpawnDistance)
        {
            KeyValuePair<GameObject, float> building = GetRandomBuilding();
            float newBuildingPosition = previousBuildingPosition + building.Value;
            building.Key.transform.position = OffsetOneValueBuilding(buildingLeftOffset, newBuildingPosition);
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

        // Spawn the start road
        Instantiate(prefabReferences.GetStartRoadPrefab(playerCount), OffsetZValue(previousChunkPosition), Quaternion.identity);

        for (int i = 0; i < maxRoadChunks; i++)
        {
            previousChunkPosition += roadMeasurementSize;
            GameObject roadChunk = Instantiate(prefabReferences.GetRoadPrefab(playerCount), OffsetZValue(previousChunkPosition), Quaternion.identity);
            roadChunk.transform.parent = this.transform;
            roadChunks.Add(roadChunk);
        }
    }

    private void CheckChunks()
    {
        if (player.position.z > previousChunkPosition + roadMeasurementSize + startPoolingRoadDistance)
        {
            previousChunkPosition += roadMeasurementSize;
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
        if ((buidlingSizes[randomIndex].Key.transform.position.z + buldingDespawnDistance) > player.position.z)
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
