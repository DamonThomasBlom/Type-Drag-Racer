using UnityEngine;
using TMPro;

public class RoadDistanceMarker : MonoBehaviour
{
    [Header("Marker Settings")]
    public float interval = 100f; // Distance between markers
    public float startDistance = 0f; // Starting position of markers
    public float endDistance = 1000f; // End position for markers
    public Vector3 offset = new Vector3(0, 0.5f, 0); // Offset above the ground

    [Header("Marker Appearance")]
    public GameObject markerPrefab; // Prefab for displaying the text
    public Color markerColor = Color.white; // Color of the text
    public float markerSize = 1f; // Size of the text

    private void Start()
    {
        GenerateMarkers();
    }

    private void GenerateMarkers()
    {
        // Ensure the marker prefab is set
        if (markerPrefab == null)
        {
            Debug.LogError("Marker Prefab (TMP_Text) is not assigned!");
            return;
        }

        // Loop through the distance and create markers
        for (float distance = startDistance; distance <= endDistance; distance += interval)
        {
            // Position the marker on the ground
            Vector3 markerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + distance) + offset;

            // Instantiate the marker
            TextMeshProUGUI markerInstance = Instantiate(markerPrefab, markerPosition, Quaternion.Euler(90, 0, 0)).GetComponentInChildren<TextMeshProUGUI>();
            markerInstance.text = $"{distance}m"; // Set the distance text
            markerInstance.color = markerColor; // Set the text color
            markerInstance.fontSize = markerSize; // Adjust the font size

            // Parent to this object for better organization
            markerInstance.transform.parent.SetParent(transform);
        }
    }
}
