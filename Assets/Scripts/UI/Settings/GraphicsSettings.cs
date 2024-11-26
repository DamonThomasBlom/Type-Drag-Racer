using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicsSettings : MonoBehaviour
{
    [Header("Post-Processing")]
    public Volume globalVolume; // Assign the global volume in your scene here

    private Bloom bloomEffect;
    private Vignette vignetteEffect;

    private const string BloomPref = "Graphics_Bloom";
    private const string VignettePref = "Graphics_Vignette";
    private const string QualityPref = "Graphics_Quality";

    private void Start()
    {
        // Initialize references to the effects in the Volume Profile
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out bloomEffect);
            globalVolume.profile.TryGet(out vignetteEffect);
        }
    }

    public void SetBloom(bool enabled)
    {
        if (bloomEffect != null)
        {
            bloomEffect.active = enabled;
        }

        PlayerPrefs.SetInt(BloomPref, enabled ? 1 : 0);
    }

    public void SetVignette(bool enabled)
    {
        if (vignetteEffect != null)
        {
            vignetteEffect.active = enabled;
        }

        PlayerPrefs.SetInt(VignettePref, enabled ? 1 : 0);
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
        PlayerPrefs.SetInt(QualityPref, level);
    }
}
