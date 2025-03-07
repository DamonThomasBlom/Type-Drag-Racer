using Sirenix.OdinInspector;
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
    public List<WheelScriptable> wheelScriptables = new List<WheelScriptable>();
    public List<Material> carMaterials = new List<Material>();
    public Dictionary<string, WheelScriptable> carDefaultWheels = new Dictionary<string, WheelScriptable>();

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

    public WheelScriptable GetWheelScriptable(string wheelName)
    {
        WheelScriptable wheelScriptable = null;

        foreach (var wheel in wheelScriptables) 
        {
            if (wheel.name == wheelName)
                wheelScriptable = wheel;
        }

        if (wheelScriptable == null)
        {
            Debug.LogError("Couldn't find a matching wheel scriptable for - " + wheelScriptable);
            wheelScriptable = wheelScriptables[0];
        }

        return wheelScriptable;
    }

    public WheelScriptable GetDefaultWheels(string carName)
    {
        if (carDefaultWheels.TryGetValue(carName, out WheelScriptable defaultWheels))
        {
            return defaultWheels;
        }
        Debug.LogWarning($"No default wheels set for car {carName}. Returning first available wheel.");
        return wheelScriptables.Count > 0 ? wheelScriptables[0] : null;
    }

    public Material GetMaterialByName(string name)
    {
        foreach(var mat in carMaterials)
        {
            if (mat.name == name)
                return mat;
        }

        Debug.LogError("Couldn't find a matching material for - " + name);
        return null;
    }

    public Material GetDefaultMaterial()
    {
        return carMaterials.Count > 0 ? carMaterials[0] : null;
    }

    public string GetRandomCarPrefabName()
    {
        // Randomly select a car name from the dictionary
        List<string> carNames = new List<string>(carDictionary.Keys);
        return carNames[Random.Range(0, carNames.Count)];
    }

    public string GetRandomWheelName()
    {
        WheelScriptable randomWheel = wheelScriptables[Random.Range(0, wheelScriptables.Count)];
        return randomWheel.name;
    }

    public string GetRandomMaterialName()
    {
        Material randomMaterial = carMaterials[Random.Range(0, carMaterials.Count)];
        return randomMaterial.name;
    }
}
