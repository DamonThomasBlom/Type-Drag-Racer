using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCar : MonoBehaviour
{
    public string CurrentMaterialName;
    public string CurrentWheelName;

    private MeshRenderer _carMeshRenderer;
    private CarWheelRotation _carWheelRotation;

    private void Awake()
    {
        _carMeshRenderer = GetComponent<MeshRenderer>();
        _carWheelRotation = GetComponent<CarWheelRotation>();
    }

    [Button]
    public void UpdateMaterials(Material mat)
    {
        if (_carMeshRenderer == null) { return; }

        _carMeshRenderer.material = mat;
        CurrentMaterialName = mat.name;
    }

    [Button]
    public void UpdateWheels(WheelScriptable wheelScriptable)
    {
        if (_carWheelRotation == null) 
            _carWheelRotation = GetComponent<CarWheelRotation>();

        // Replace left wheels
        ReplaceWheels(_carWheelRotation.LeftWheels, wheelScriptable.LeftWheel, true);

        // Replace right wheels
        ReplaceWheels(_carWheelRotation.RightWheels, wheelScriptable.RightWheel, false);

        CurrentWheelName = wheelScriptable.name;
    }

    void ReplaceWheels(List<Transform> currentWheels, GameObject newWheel, bool leftWheel)
    {
        List<GameObject> destroyList = new List<GameObject>();
        List<GameObject> newWheelList = new List<GameObject>();

        // Instantiate the new wheels in the same spots as the old wheels
        foreach (Transform currentWheel in currentWheels)
        {
            destroyList.Add(currentWheel.gameObject);
            newWheelList.Add(Instantiate(newWheel, currentWheel.position, currentWheel.rotation, currentWheel.parent));
        }

        // Assign the new wheels to the script that handles rotating the wheels
        if (leftWheel)
        {
            _carWheelRotation.LeftWheels.Clear();
            foreach (var newWheelInstance in newWheelList) { _carWheelRotation.LeftWheels.Add(newWheelInstance.transform); }
        }
        else
        {
            _carWheelRotation.RightWheels.Clear();
            foreach (var newWheelInstance in newWheelList) { _carWheelRotation.RightWheels.Add(newWheelInstance.transform); }
        }

        // Destroy the old wheels
        foreach (var oldWheel in destroyList)
        {
            DestroyImmediate(oldWheel);
        }
    }

    [Button]
    public void RandomizeCar()
    {
        // Randomize car material
        if (PrefabManager.Instance.carMaterials.Count > 0 && _carMeshRenderer != null)
        {
            Material randomMaterial = PrefabManager.Instance.GetMaterialByName(PrefabManager.Instance.GetRandomMaterialName());
            UpdateMaterials(randomMaterial);
        }

        // Randomize wheels
        if (PrefabManager.Instance.wheelScriptables.Count > 0)
        {
            WheelScriptable randomWheel = PrefabManager.Instance.GetWheelScriptable(PrefabManager.Instance.GetRandomWheelName());
            UpdateWheels(randomWheel);
        }
    }
}