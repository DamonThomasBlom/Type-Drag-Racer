using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Debug")]
    [Space(10)]
    public int playerCount = 2;

    [Header("Road Settings")]
    public bool InitOnStart = true;
    [Tooltip("Max pooled roads")]
    public int maxRoadChunks = 10;
    [Tooltip("Road offset distance")]
    public float roadMeasurementSize = 5;
    [Tooltip("When should we start pooling")]
    public float startPoolingRoadDistance = 20;

    [Header("World Chunk Settings")]
    public bool disableWorldGeneration;
    [Tooltip("Max pooled buildings")]
    public int maxBlocks = 20;
    [Tooltip("Building spawn distance from player")]
    public float blockSpawnDistance = 100;
    [Tooltip("Building despawn distance")]
    public float blockDespawnDistance = 10;
    public List<WorldBlockDefinition> worldBlocks = new List<WorldBlockDefinition>();

    [Space(10f)]
    public Transform player;

    [Space(10f)]
    public PrefabReferences prefabReferences;

    private List<GameObject> roadChunks = new List<GameObject>();
    private List<GameObject> PooledWorldBlocksLeft = new List<GameObject>();
    private List<GameObject> PooledWorldBlocksRight = new List<GameObject>();

    private const float ROAD_WIDTH = 5;
    private float previousChunkPosition;
    private float previousBlockPosition;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (player == null)
        {
            if (Player.Instance.PlayerTransform == null)
            {
                Invoke(nameof(Init), 1);
                return;
            }
            player = Player.Instance.PlayerTransform;
        }

        SetupRoadChunks();
        SetupWorldBlocks();
    }

    private void Update()
    {
        if (player == null) { return; }
        CheckRoadChunks();
        CheckWorldBlocks();
    }

    public void SetupFinishLine(float distance)
    {
        distance -= 5;
        // Spawn the finish line road
        Instantiate(prefabReferences.GetFinishLineRoadPrefab(playerCount), OffsetZValue(distance), Quaternion.identity);
    }

    private void SetupWorldBlocks()
    {
        if (disableWorldGeneration) return;

        previousBlockPosition = -1000f; // start behind player
        float blockWidth = worldBlocks[0].blockWidth;
        blockSpawnDistance += blockWidth;
        blockDespawnDistance += blockWidth;

        for (int i = 0; i < maxBlocks; i++)
        {
            foreach (var blockDef in worldBlocks)
            {
                // Left pooling
                if (blockDef.spawnSide == WorldBlockDefinition.SpawnSide.Left)
                {
                    var block = Instantiate(blockDef.prefab, new Vector3(0, 0, -blockWidth * maxBlocks), Quaternion.identity, transform);
                    PooledWorldBlocksLeft.Add(block);
                }

                // Right pooling
                if (blockDef.spawnSide == WorldBlockDefinition.SpawnSide.Right)
                {
                    var block = Instantiate(blockDef.prefab, new Vector3(ROAD_WIDTH * playerCount, 0, -blockWidth * maxBlocks), Quaternion.identity, transform);
                    PooledWorldBlocksRight.Add(block);
                }
            }
        }
    }

    private void CheckWorldBlocks()
    {
        if (disableWorldGeneration) { return; }
        if (previousBlockPosition < player.position.z + blockSpawnDistance)
        {
            // When player reaches spawn threshold, generate a new line of blocks
            if (previousBlockPosition < player.position.z + blockSpawnDistance)
            {
                // Move forward by the width of ONE block
                previousBlockPosition += worldBlocks[0].blockWidth;

                SpawnNextBlockLine(previousBlockPosition);
            }
        }
    }

    private void SpawnNextBlockLine(float zPos)
    {
        GameObject left = GetAvailableBlock(PooledWorldBlocksLeft);
        GameObject right = GetAvailableBlock(PooledWorldBlocksRight);

        if (left != null)
            left.transform.position = new Vector3(0, 0, zPos);

        if (right != null)
            right.transform.position = new Vector3(ROAD_WIDTH * playerCount, 0, zPos);
    }

    private GameObject GetAvailableBlock(List<GameObject> pool)
    {
        foreach (var block in pool)
        {
            // Only choose blocks that are behind player (ready for reuse)
            if (block.transform.position.z + blockDespawnDistance < player.position.z)
                return block;
        }

        return null;
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

    private void CheckRoadChunks()
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

    private GameObject GetRandomLeftBlock()
    {
        if (PooledWorldBlocksLeft.Count == 0)
        {
            Debug.LogError("No Left Blocks");
            return null;
        }

        int randomIndex = Random.Range(0, PooledWorldBlocksLeft.Count - 1);

        // Check that our block isn't infront of our player
        float currentBlockZValue = PooledWorldBlocksLeft[randomIndex].transform.position.z + blockDespawnDistance;
        if (currentBlockZValue > player.position.z)
            return GetRandomLeftBlock();

        return PooledWorldBlocksLeft[randomIndex];
    }

    private GameObject GetRandomRightBlock()
    {
        if (PooledWorldBlocksRight.Count == 0)
        {
            Debug.LogError("No Right Blocks");
            return null;
        }

        int randomIndex = Random.Range(0, PooledWorldBlocksRight.Count - 1);

        // Check that our block isn't infront of our player
        float currentBlockZValue = PooledWorldBlocksRight[randomIndex].transform.position.z + blockDespawnDistance;
        if (currentBlockZValue > player.position.z)
            return GetRandomRightBlock();

        return PooledWorldBlocksRight[randomIndex];
    }

    private Vector3 OffsetZValue(float offset)
    {
        return new Vector3(0, 0, offset);
    }
}
