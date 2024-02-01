using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabReferences", menuName = "Custom/Prefab References")]
public class PrefabReferences : ScriptableObject
{
    public List<GameObject> buildings;
    public GameObject startRoadPrefab2;
    public GameObject startRoadPrefab4;
    public GameObject startRoadPrefab6;
    public GameObject startRoadPrefab8;
    public GameObject roadPrefab2;
    public GameObject roadPrefab4;
    public GameObject roadPrefab6;
    public GameObject roadPrefab8;
    public GameObject finishLineRoadPrefab2;
    public GameObject finishLineRoadPrefab4;
    public GameObject finishLineRoadPrefab6;
    public GameObject finishLineRoadPrefab8;

    public GameObject GetStartRoadPrefab(int size)
    {
        switch (size)
        {
            case 2:
                return startRoadPrefab2;
            case 4:
                return startRoadPrefab4;
            case 6:
                return startRoadPrefab6;
            case 8:
                return startRoadPrefab8;
            default:
                return null;
        }
    }

    public GameObject GetRoadPrefab(int size)
    {
        switch (size)
        {
            case 2:
                return roadPrefab2;
            case 4:
                return roadPrefab4;
            case 6:
                return roadPrefab6;
            case 8:
                return roadPrefab8;
            default:
                return null;
        }
    }

    public GameObject GetFinishLineRoadPrefab(int size)
    {
        switch (size)
        {
            case 2:
                return finishLineRoadPrefab2;
            case 4:
                return finishLineRoadPrefab4;
            case 6:
                return finishLineRoadPrefab6;
            case 8:
                return finishLineRoadPrefab8;
            default:
                return null;
        }
    }
}
