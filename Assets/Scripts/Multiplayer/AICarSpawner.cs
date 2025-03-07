using Fusion;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICarSpawner : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(SpawnCar))]
    public string carPrefabName {  get; set; }

    [Networked, OnChangedRender(nameof(UpdateAIMaterial)), Capacity(50)]
    public string carMaterialName { get; set; }

    [Networked, OnChangedRender(nameof(UpdateAIWheels)), Capacity(50)]
    public string carWheelName { get; set; }

    public Transform spawnPoint;

    private GameObject _carInstance;
    private CustomizeCar _customizeCar;

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            SpawnRandomCar();
        }
        else 
        {
            SpawnCar();
        }
    }

    [Button]
    void SpawnRandomCar()
    {
        // Check if there are any car prefabs in the dictionary
        if (PrefabManager.Instance.carDictionary.Count == 0)
        {
            Debug.LogWarning("No car prefabs assigned to the dictionary.");
            return;
        }

        carPrefabName = PrefabManager.Instance.GetRandomCarPrefabName();
        carMaterialName = PrefabManager.Instance.GetRandomMaterialName();
        carWheelName = PrefabManager.Instance.GetRandomWheelName();

        InstantiateCar(carPrefabName);
    }

    void SpawnCar()
    {
        if (!string.IsNullOrEmpty(carPrefabName)) 
        {
            InstantiateCar(carPrefabName);
        }
    }

    void UpdateAIMaterial()
    {
        if (!string.IsNullOrEmpty(carMaterialName) && _customizeCar != null)
        {
            _customizeCar.UpdateMaterials(PrefabManager.Instance.GetMaterialByName(carMaterialName));
        }
    }

    void UpdateAIWheels()
    {
        if (!string.IsNullOrEmpty(carWheelName) && _customizeCar != null)
        {
            _customizeCar.UpdateWheels(PrefabManager.Instance.GetWheelScriptable(carWheelName));
        }
    }

    void InstantiateCar(string carName)
    {
        if (_carInstance != null) { return; }

        var selectedCarPrefab = PrefabManager.Instance.GetCarPrefab(carName);
        _carInstance = Instantiate(selectedCarPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        _customizeCar = _carInstance.GetComponent<CustomizeCar>();

        if (!string.IsNullOrEmpty(carMaterialName) && _customizeCar != null)
        {
            _customizeCar.UpdateMaterials(PrefabManager.Instance.GetMaterialByName(carMaterialName));
        }

        if (!string.IsNullOrEmpty(carWheelName) && _customizeCar != null)
        {
            _customizeCar.UpdateWheels(PrefabManager.Instance.GetWheelScriptable(carWheelName));
        }
    }
}
