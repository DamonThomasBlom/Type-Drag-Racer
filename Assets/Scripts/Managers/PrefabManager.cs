using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : SerializedMonoBehaviour
{
    private static PrefabManager _instance;

    public static PrefabManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject repositoryObject = new GameObject("PrefabRepository");
                _instance = repositoryObject.AddComponent<PrefabManager>();
            }
            return _instance;
        }
    }

    public Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> carDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        // Ensure there's only one instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void AddPrefab(string prefabName, GameObject prefab)
    {
        if (!prefabDictionary.ContainsKey(prefabName))
        {
            prefabDictionary.Add(prefabName, prefab);
        }
        else
        {
            Debug.LogWarning($"Prefab with name '{prefabName}' already exists in the repository.");
        }
    }

    public GameObject GetPrefab(string prefabName)
    {
        GameObject prefab;
        if (prefabDictionary.TryGetValue(prefabName, out prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"Prefab with name '{prefabName}' not found in the repository.");
            return null;
        }
    }

    public GameObject GetCarPrefab(string prefabName)
    {
        GameObject prefab;
        if (carDictionary.TryGetValue(prefabName, out prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"Prefab with name '{prefabName}' not found in the repository.");
            return null;
        }
    }
}
