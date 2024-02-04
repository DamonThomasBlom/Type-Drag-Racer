using Fusion;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICarSpawner : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(SpawnCar))]
    public string carPrefabName {  get; set; }

    public Transform spawnPoint;

    private GameObject carInstance;

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            // Delay the spawn of the car
            //Invoke(nameof(SpawnRandomCar), 1f);
            SpawnRandomCar();
        }
        else 
        {
            SpawnCar();
        }
    }

    //[Rpc(RpcSources.All, RpcTargets.All)]
    //public void SpawnRandomCarRpc(string carName, RpcInfo info = default)
    //{
    //    Debug.Log("Spawn Car RPC: " + carName);
    //    var selectedCarPrefab = PrefabManager.Instance.GetCarPrefab(carName);
    //    carInstance = Instantiate(selectedCarPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    //}

    [Button]
    void SpawnRandomCar()
    {
        // Check if there are any car prefabs in the dictionary
        if (PrefabManager.Instance.carDictionary.Count == 0)
        {
            Debug.LogWarning("No car prefabs assigned to the dictionary.");
            return;
        }

        // Randomly select a car name from the dictionary
        List<string> carNames = new List<string>(PrefabManager.Instance.carDictionary.Keys);
        string randomCarName = carNames[Random.Range(0, carNames.Count)];

        carPrefabName = randomCarName;
        //SpawnRandomCarRpc(randomCarName);
        InstantiateCar(carPrefabName);
    }

    void SpawnCar()
    {
        if (!string.IsNullOrEmpty(carPrefabName)) 
        {
            InstantiateCar(carPrefabName);
        }
    }

    void InstantiateCar(string carName)
    {
        if (carInstance != null) { return; }

        var selectedCarPrefab = PrefabManager.Instance.GetCarPrefab(carName);
        carInstance = Instantiate(selectedCarPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    }
}
