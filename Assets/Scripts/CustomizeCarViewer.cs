using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCarViewer : MonoBehaviour
{
    public Transform CarSpawnPoint;
    private GameObject currentCar;
    private int currentCarIndex;

    private string currentMaterialName;
    private string currentWheelName;

    private const string CarKey = "SelectedCar";
    private const string MaterialKey = "SelectedMaterial";
    private const string WheelKey = "SelectedWheel";

    private void Start()
    {
        LoadCustomization();
    }

    [Button]
    public void NextCar() => SwitchCar(1);
    [Button]
    public void PreviousCar() => SwitchCar(-1);

    private void SwitchCar(int direction)
    {
        if (currentCar != null) Destroy(currentCar);

        var carNames = new List<string>(PrefabManager.Instance.carDictionary.Keys);
        if (carNames.Count == 0) return;

        currentCarIndex = (currentCarIndex + direction + carNames.Count) % carNames.Count;
        string carName = carNames[currentCarIndex];

        currentCar = Instantiate(PrefabManager.Instance.GetCarPrefab(carName), CarSpawnPoint.position, CarSpawnPoint.rotation, CarSpawnPoint);
        CustomizeCar customizeCar = currentCar.GetComponent<CustomizeCar>();

        // Load saved customizations or apply defaults
        if (PlayerPrefs.HasKey($"{carName}_{MaterialKey}"))
        {
            currentMaterialName = PlayerPrefs.GetString($"{carName}_{MaterialKey}");
            Material savedMaterial = PrefabManager.Instance.GetMaterialByName(currentMaterialName);
            if (savedMaterial) customizeCar.UpdateMaterials(savedMaterial);
        }
        else
        {
            Material defaultMaterial = PrefabManager.Instance.GetDefaultMaterial();
            customizeCar.UpdateMaterials(defaultMaterial);
            currentMaterialName = defaultMaterial.name;
        }

        if (PlayerPrefs.HasKey($"{carName}_{WheelKey}"))
        {
            currentWheelName = PlayerPrefs.GetString($"{carName}_{WheelKey}");
            WheelScriptable savedWheel = PrefabManager.Instance.GetWheelScriptable(currentWheelName);
            if (savedWheel != null) customizeCar.UpdateWheels(savedWheel);
        }
        else
        {
            WheelScriptable defaultWheels = PrefabManager.Instance.GetDefaultWheels(carName);
            customizeCar.UpdateWheels(defaultWheels);
            currentWheelName = defaultWheels.name;
        }

        PlayerPrefs.SetString(CarKey, carName);
        Player.Instance.CarName = carName;
        Player.Instance.MaterialName = currentMaterialName;
        Player.Instance.WheelName = currentWheelName;
        PlayerPrefs.Save();
    }

    [Button]
    public void NextMaterial()
    {
        if (currentCar == null) return;

        var materials = PrefabManager.Instance.carMaterials;
        if (materials.Count == 0) return;

        int newIndex = (materials.FindIndex(m => m.name == currentMaterialName) + 1) % materials.Count;
        Material newMaterial = materials[newIndex];

        currentCar.GetComponent<CustomizeCar>().UpdateMaterials(newMaterial);
        currentMaterialName = newMaterial.name;

        SaveCustomization();
    }

    [Button]
    public void PreviousMaterial()
    {
        if (currentCar == null) return;

        var materials = PrefabManager.Instance.carMaterials;
        if (materials.Count == 0) return;

        int newIndex = (materials.FindIndex(m => m.name == currentMaterialName) - 1 + materials.Count) % materials.Count;
        Material newMaterial = materials[newIndex];

        currentCar.GetComponent<CustomizeCar>().UpdateMaterials(newMaterial);
        currentMaterialName = newMaterial.name;

        SaveCustomization();
    }

    [Button]
    public void NextWheel()
    {
        if (currentCar == null) return;

        var wheels = PrefabManager.Instance.wheelScriptables;
        if (wheels.Count == 0) return;

        int newIndex = (wheels.FindIndex(w => w.name == currentWheelName) + 1) % wheels.Count;
        WheelScriptable newWheel = wheels[newIndex];

        currentCar.GetComponent<CustomizeCar>().UpdateWheels(newWheel);
        currentWheelName = newWheel.name;

        SaveCustomization();
    }

    [Button]
    public void PreviousWheel()
    {
        if (currentCar == null) return;

        var wheels = PrefabManager.Instance.wheelScriptables;
        if (wheels.Count == 0) return;

        int newIndex = (wheels.FindIndex(w => w.name == currentWheelName) - 1 + wheels.Count) % wheels.Count;
        WheelScriptable newWheel = wheels[newIndex];

        currentCar.GetComponent<CustomizeCar>().UpdateWheels(newWheel);
        currentWheelName = newWheel.name;

        SaveCustomization();
    }

    [Button]
    public void RandomizeAll()
    {
        if (PrefabManager.Instance.carDictionary.Count == 0 || PrefabManager.Instance.carMaterials.Count == 0 || PrefabManager.Instance.wheelScriptables.Count == 0)
            return;

        string randomCarName = PrefabManager.Instance.GetRandomCarPrefabName();
        string randomMaterialName = PrefabManager.Instance.GetRandomMaterialName();
        string randomWheelName = PrefabManager.Instance.GetRandomWheelName();

        currentCarIndex = new List<string>(PrefabManager.Instance.carDictionary.Keys).IndexOf(randomCarName);
        SwitchCar(0);

        Material randomMaterial = PrefabManager.Instance.GetMaterialByName(randomMaterialName);
        WheelScriptable randomWheel = PrefabManager.Instance.GetWheelScriptable(randomWheelName);

        if (currentCar != null)
        {
            currentCar.GetComponent<CustomizeCar>().UpdateMaterials(randomMaterial);
            currentCar.GetComponent<CustomizeCar>().UpdateWheels(randomWheel);
        }

        currentMaterialName = randomMaterialName;
        currentWheelName = randomWheelName;

        SaveCustomization();
    }

    [Button]
    public void ClearAllCarSettings()
    {
        foreach (var carName in PrefabManager.Instance.carDictionary.Keys)
        {
            PlayerPrefs.DeleteKey($"{carName}_{MaterialKey}");
            PlayerPrefs.DeleteKey($"{carName}_{WheelKey}");
        }
        PlayerPrefs.Save();

        // Load the default car
        LoadDefaultCar();
    }

    private void SaveCustomization()
    {
        string carName = PlayerPrefs.GetString(CarKey, "");
        if (string.IsNullOrEmpty(carName)) return;

        PlayerPrefs.SetString($"{carName}_{MaterialKey}", currentMaterialName);
        PlayerPrefs.SetString($"{carName}_{WheelKey}", currentWheelName);
        PlayerPrefs.Save();

        Player.Instance.MaterialName = currentMaterialName;
        Player.Instance.WheelName = currentWheelName;
    }

    private void LoadCustomization()
    {
        string savedCar = PlayerPrefs.GetString(CarKey, "");

        if (!string.IsNullOrEmpty(savedCar) && PrefabManager.Instance.GetCarPrefab(savedCar) != null)
        {
            currentCarIndex = new List<string>(PrefabManager.Instance.carDictionary.Keys).IndexOf(savedCar);
            SwitchCar(0);
        }
        else
        {
            LoadDefaultCar();
        }
    }

    void LoadDefaultCar()
    {
        currentCarIndex = 5;
        SwitchCar(0);
    }
}
