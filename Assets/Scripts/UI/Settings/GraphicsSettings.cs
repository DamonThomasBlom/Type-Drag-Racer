using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.EventSystems;

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fpsDropdown;
    public Button lowQualityButton;
    public Button mediumQualityButton;
    public Button highQualityButton;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle playerNamesToggle;

    [Header("Audio")]
    public AudioMixer audioMixer; // Unity's Audio Mixer

    private Resolution[] availableResolutions;

    private const string ResolutionPref = "Graphics_Resolution";
    private const string FPSPref = "Graphics_FPS";
    private const string QualityPref = "Graphics_Quality";
    private const string MasterVolumePref = "Audio_MasterVolume";
    private const string MusicVolumePref = "Audio_MusicVolume";
    private const string SFXVolumePref = "Audio_SFXVolume";
    private const string PlayerNamesPref = "Gameplay_ShowPlayerNames";

    private void Start()
    {
        InitializeResolutionDropdown();
        InitializeFPSDropdown();
        InitializeGraphicsQualityButtons();
        InitializeVolumeSliders();
        InitializePlayerNamesToggle();
    }

    private void InitializeResolutionDropdown()
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        foreach (var res in availableResolutions)
            options.Add($"{res.width} x {res.height}");

        resolutionDropdown.AddOptions(options);

        int savedIndex = PlayerPrefs.GetInt(ResolutionPref, GetCurrentResolutionIndex());
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        ApplyResolution(savedIndex);
        resolutionDropdown.onValueChanged.AddListener((val) => ApplyResolution(val));
    }

    private void InitializeFPSDropdown()
    {
        fpsDropdown.ClearOptions();
        var fpsOptions = new[] { 30, 60, 120, 144, 165, 240, -1 }; // -1 means uncapped
        var options = new System.Collections.Generic.List<string>();

        foreach (int fps in fpsOptions)
            options.Add(fps == -1 ? "Unlimited" : $"{fps} FPS");

        fpsDropdown.AddOptions(options);

        int savedFPS = PlayerPrefs.GetInt(FPSPref, -1);
        fpsDropdown.value = System.Array.IndexOf(fpsOptions, savedFPS);
        fpsDropdown.RefreshShownValue();

        ApplyFPSLimit(fpsDropdown.value);
        fpsDropdown.onValueChanged.AddListener((val) => ApplyFPSLimit(val));
    }

    private void InitializeGraphicsQualityButtons()
    {
        lowQualityButton.onClick.AddListener(() => SetGraphicsQuality(0)); // Low
        mediumQualityButton.onClick.AddListener(() => SetGraphicsQuality(1)); // Medium
        highQualityButton.onClick.AddListener(() => SetGraphicsQuality(2)); // High

        int savedQuality = PlayerPrefs.GetInt(QualityPref, 2); // Default to High
        SetGraphicsQuality(savedQuality);
    }

    private void InitializeVolumeSliders()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolumePref, 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolumePref, 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFXVolumePref, 1f);

        ApplyVolume(MasterVolumePref, masterVolumeSlider.value, "Master");
        ApplyVolume(MusicVolumePref, musicVolumeSlider.value, "Music");
        ApplyVolume(SFXVolumePref, sfxVolumeSlider.value, "SFX");

        masterVolumeSlider.onValueChanged.AddListener(val => ApplyVolume(MasterVolumePref, val, "Master"));
        musicVolumeSlider.onValueChanged.AddListener(val => ApplyVolume(MusicVolumePref, val, "Music"));
        sfxVolumeSlider.onValueChanged.AddListener(val => ApplyVolume(SFXVolumePref, val, "SFX"));
    }

    private void InitializePlayerNamesToggle()
    {
        bool showPlayerNames = PlayerPrefs.GetInt(PlayerNamesPref, 1) == 1;
        playerNamesToggle.isOn = showPlayerNames;

        playerNamesToggle.onValueChanged.AddListener(val =>
        {
            PlayerPrefs.SetInt(PlayerNamesPref, val ? 1 : 0);
            Debug.Log("Player names visibility toggled: " + val);
        });
    }

    private void ApplyResolution(int index)
    {
        if (index < 0 || index >= availableResolutions.Length) return;

        Resolution res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt(ResolutionPref, index);
    }

    private void ApplyFPSLimit(int index)
    {
        int[] fpsOptions = { 30, 60, 120, 144, 240, -1 };
        if (index < 0 || index >= fpsOptions.Length) return;

        int fpsLimit = fpsOptions[index];
        Application.targetFrameRate = fpsLimit;
        PlayerPrefs.SetInt(FPSPref, fpsLimit);
    }

    #region Quality Settings

    private void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(QualityPref, qualityIndex);
        Debug.Log("Graphics quality set to: " + QualitySettings.names[qualityIndex]);

        DeselectButton();
        UpdateQualityButtons();
    }

    private void UpdateQualityButtons()
    {
        // Reset all buttons to default state
        ResetButtonStyles();

        // Highlight the current quality level button
        int currentQuality = QualitySettings.GetQualityLevel();

        switch (currentQuality)
        {
            case 0:
                HighlightButton(lowQualityButton);
                break;
            case 1:
                HighlightButton(mediumQualityButton);
                break;
            case 2:
                HighlightButton(highQualityButton);
                break;
        }
    }

    private void ResetButtonStyles()
    {
        SetButtonDefaultStyle(lowQualityButton);
        SetButtonDefaultStyle(mediumQualityButton);
        SetButtonDefaultStyle(highQualityButton);
    }

    private void HighlightButton(Button button)
    {
        // Example: Change the background color to highlight the button
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = Color.green; // Highlight color
        button.colors = colorBlock;

        // Optional: Bold text
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.fontStyle = FontStyle.Bold;
        }
    }

    private void SetButtonDefaultStyle(Button button)
    {
        // Reset to default background color
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = Color.white; // Default color
        button.colors = colorBlock;

        // Optional: Normal text style
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.fontStyle = FontStyle.Normal;
        }
    }

    private void DeselectButton()
    {
        // Force deselection of the currently selected UI element
        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion

    private void ApplyVolume(string prefKey, float value, string mixerParameter)
    {
        audioMixer.SetFloat(mixerParameter, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat(prefKey, value);
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (Screen.currentResolution.width == availableResolutions[i].width &&
                Screen.currentResolution.height == availableResolutions[i].height)
                return i;
        }
        return 0;
    }
}
