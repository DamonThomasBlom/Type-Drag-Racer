using UnityEngine;

public class GraphicsSettingsLoader : MonoBehaviour
{
    private const string BloomPref = "Graphics_Bloom";
    private const string VignettePref = "Graphics_Vignette";
    private const string QualityPref = "Graphics_Quality";

    [SerializeField] private GraphicsSettings graphicsSettings;

    private void Awake()
    {
        //// Load Bloom setting
        //bool bloomEnabled = PlayerPrefs.GetInt(BloomPref, 1) == 1; // Default is enabled
        //graphicsSettings.SetBloom(bloomEnabled);

        //// Load Vignette setting
        //bool vignetteEnabled = PlayerPrefs.GetInt(VignettePref, 1) == 1; // Default is enabled
        //graphicsSettings.SetVignette(vignetteEnabled);

        //// Load Quality Level
        //int qualityLevel = PlayerPrefs.GetInt(QualityPref, QualitySettings.GetQualityLevel());
        //graphicsSettings.SetQualityLevel(qualityLevel);
    }
}
