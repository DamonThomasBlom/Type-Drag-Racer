using Michsky.MUIP;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CustomizePanelUI : MonoBehaviour
{
    [FoldoutGroup("Car Selection")]
    public ButtonManager nextCarButton;
    [FoldoutGroup("Car Selection")]
    public ButtonManager previousCarButton;

    [FoldoutGroup("Material Selection")]
    public ButtonManager nextMaterialButton;
    [FoldoutGroup("Material Selection")]
    public ButtonManager previousMaterialButton;

    [FoldoutGroup("Wheel Selection")]
    public ButtonManager nextWheelButton;
    [FoldoutGroup("Wheel Selection")]
    public ButtonManager previousWheelButton;

    [FoldoutGroup("Actions")]
    public ButtonManager randomizeButton;
    [FoldoutGroup("Actions")]
    public ButtonManager resetButton;

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
