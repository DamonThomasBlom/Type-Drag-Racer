using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CustomizePanelUI : MonoBehaviour
{
    [FoldoutGroup("Car Selection")]
    public Button nextCarButton;
    [FoldoutGroup("Car Selection")]
    public Button previousCarButton;

    [FoldoutGroup("Material Selection")]
    public Button nextMaterialButton;
    [FoldoutGroup("Material Selection")]
    public Button previousMaterialButton;

    [FoldoutGroup("Wheel Selection")]
    public Button nextWheelButton;
    [FoldoutGroup("Wheel Selection")]
    public Button previousWheelButton;

    [FoldoutGroup("Actions")]
    public Button randomizeButton;
    [FoldoutGroup("Actions")]
    public Button resetButton;

    private CustomizeCarViewer carViewer;

    private void Start()
    {
        carViewer = FindObjectOfType<CustomizeCarViewer>();

        nextCarButton.onClick.AddListener(() => carViewer.NextCar());
        previousCarButton.onClick.AddListener(() => carViewer.PreviousCar());

        nextMaterialButton.onClick.AddListener(() => carViewer.NextMaterial());
        previousMaterialButton.onClick.AddListener(() => carViewer.PreviousMaterial());

        nextWheelButton.onClick.AddListener(() => carViewer.NextWheel());
        previousWheelButton.onClick.AddListener(() => carViewer.PreviousWheel());

        randomizeButton.onClick.AddListener(() => carViewer.RandomizeAll());
        resetButton.onClick.AddListener(() => carViewer.ClearAllCarSettings());
    }
}
