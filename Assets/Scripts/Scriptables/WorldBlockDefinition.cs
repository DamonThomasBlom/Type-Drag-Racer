using UnityEngine;

[CreateAssetMenu(fileName = "WorldBlockDefinition", menuName = "ScriptableObjects/Block Definition")]
public class WorldBlockDefinition : ScriptableObject
{
    public GameObject prefab;

    [Tooltip("Width of the block along X")]
    public float blockWidth;

    [Tooltip("Spawn on left, right, or both sides of the road")]
    public SpawnSide spawnSide = SpawnSide.Left;

    public enum SpawnSide { Left, Right }
}
