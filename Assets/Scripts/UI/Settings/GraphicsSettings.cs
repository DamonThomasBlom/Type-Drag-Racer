using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using Michsky.MUIP;
using UnityEngine.Events;
using System;
using TMPro;

[System.Serializable]
public class QualityButtonGroup 
{
    public Image ButtonImage;
    public TextMeshProUGUI ButtonText;
}

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI References")]
    public CustomDropdown resolutionDropdown;
    public CustomDropdown fpsDropdown;
    public ButtonManager lowQualityButton;
    public ButtonManager mediumQualityButton;
    public ButtonManager highQualityButton;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle playerNamesToggle;
    public Toggle showTypingStatsToggle;
    public Toggle pingToggle;
    public Toggle fpsToggle;

    [Header("Quality Buttons Settings")]
    public QualityButtonGroup lowQualityButtonGroup;
    public QualityButtonGroup mediumQualityButtonGroup;
    public QualityButtonGroup highQualityButtonGroup;
    public Color selectedColour;
    public Color unselectedColour;

    [Header("Audio")]
    public AudioMixer audioMixer; // Unity's Audio Mixer
    public AudioMixerGroup group;

    private List<Resolution> availableResolutions;

    public const string ResolutionPref = "Graphics_Resolution";
    public const string FPSPref = "Graphics_FPS";
    public const string QualityPref = "Graphics_Quality";
    public const string MasterVolumePref = "Audio_MasterVolume";
    public const string MusicVolumePref = "Audio_MusicVolume";
    public const string SFXVolumePref = "Audio_SFXVolume";
    public const string PlayerNamesPref = "Gameplay_ShowPlayerNames";
    public const string TypingStatsPref = "Gameplay_ShowTypingStats";
    public const string ShowPingPref = "Gameplay_ShowPingStats";
    public const string ShowFPSPref = "Gameplay_ShowFPSStats";

    private void Start()
    {
        InitializeResolutionDropdown();
        InitializeFPSDropdown();
        InitializeGraphicsQualityButtons();
        InitializeVolumeSliders();
        InitializeToggle(playerNamesToggle, PlayerNamesPref);
        InitializeToggle(showTypingStatsToggle, TypingStatsPref);
        InitializeToggle(pingToggle, ShowPingPref);
        InitializeToggle(fpsToggle, ShowFPSPref);
    }

    private void InitializeResolutionDropdown()
    {
        availableResolutions = Screen.resolutions
            .Select(res => new Resolution { width = res.width, height = res.height })  // Ignore refresh rate
            .Distinct()
            .ToList();

        resolutionDropdown.items.Clear();

        var options = new List<CustomDropdown.Item>();
        foreach (var res in availableResolutions)
        {
            var item = new CustomDropdown.Item
            {
                itemName = $"{res.width} x {res.height}",
                OnItemSelection = new UnityEvent()
            };

            options.Add(item);
        }

        resolutionDropdown.items.AddRange(options);

        int savedIndex = PlayerPrefs.GetInt(ResolutionPref, GetCurrentResolutionIndex());
        resolutionDropdown.selectedItemIndex = savedIndex;

        ApplyResolution(savedIndex);
        resolutionDropdown.onValueChanged.AddListener((val) => ApplyResolution(val));

        resolutionDropdown.SetupDropdown();
    }

    private void InitializeFPSDropdown()
    {
        fpsDropdown.items.Clear();
        var fpsOptions = new[] { 30, 60, 120, 144, 165, 240, -1 }; // -1 means uncapped
        var options = new List<CustomDropdown.Item>();

        foreach (int fps in fpsOptions)
        {
            var item = new CustomDropdown.Item
            {
                itemName = fps == -1 ? "Unlimited" : $"{fps} FPS",
                OnItemSelection = new UnityEvent()
            };

            options.Add(item);
        }

        fpsDropdown.items.AddRange(options);

        int savedFPS = PlayerPrefs.GetInt(FPSPref, -1);
        fpsDropdown.selectedItemIndex = Array.IndexOf(fpsOptions, savedFPS);

        ApplyFPSLimit(fpsDropdown.selectedItemIndex);
        fpsDropdown.onValueChanged.AddListener((val) => ApplyFPSLimit(val));

        fpsDropdown.SetupDropdown();
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

    private void InitializeToggle(Toggle toggle, string playerPref)
    {
        bool playerPrefSetting = PlayerPrefs.GetInt(playerPref, 1) == 1;
        toggle.isOn = playerPrefSetting;
        UpdateIngameSetting(playerPref, playerPrefSetting);

        toggle.onValueChanged.AddListener(val =>
        {
            UpdateIngameSetting(playerPref, val);
            PlayerPrefs.SetInt(playerPref, val ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"{toggle.name} toggled: " + val);
        });
    }

    void UpdateIngameSetting(string pref, bool settting)
    {
        switch (pref)
        {
            case PlayerNamesPref: Player.Instance.InGameSettings.ShowPlayerNames = settting; break;
            case TypingStatsPref: Player.Instance.InGameSettings.ShowTypingStats = settting; break;
            case ShowPingPref: Player.Instance.InGameSettings.ShowPing = settting; break;
            case ShowFPSPref: Player.Instance.InGameSettings.ShowFPS = settting; break;
        }
    }

    private void ApplyResolution(int index)
    {
        if (index < 0 || index >= availableResolutions.Count) return;

        Resolution res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt(ResolutionPref, index);
        PlayerPrefs.Save();
    }

    private void ApplyFPSLimit(int index)
    {
        int[] fpsOptions = { 30, 60, 120, 144, 165, 240, -1 };
        if (index < 0 || index >= fpsOptions.Length) return;

        int fpsLimit = fpsOptions[index];
        Application.targetFrameRate = fpsLimit;
        PlayerPrefs.SetInt(FPSPref, fpsLimit);
        PlayerPrefs.Save();
    }

    #region Quality Settings

    private void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(QualityPref, qualityIndex);
        PlayerPrefs.Save();
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
                HighlightButton(lowQualityButtonGroup);
                break;
            case 1:
                HighlightButton(mediumQualityButtonGroup);
                break;
            case 2:
                HighlightButton(highQualityButtonGroup);
                break;
        }
    }

    private void ResetButtonStyles()
    {
        SetButtonDefaultStyle(lowQualityButtonGroup);
        SetButtonDefaultStyle(mediumQualityButtonGroup);
        SetButtonDefaultStyle(highQualityButtonGroup);
    }

    private void HighlightButton(QualityButtonGroup button)
    {
        button.ButtonImage.color = selectedColour;
        button.ButtonText.fontStyle = (FontStyles)FontStyle.Bold;
    }

    private void SetButtonDefaultStyle(QualityButtonGroup button)
    {
        button.ButtonImage.color = unselectedColour;
        button.ButtonText.fontStyle = (FontStyles)FontStyle.Normal;
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
        PlayerPrefs.Save();
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            if (Screen.currentResolution.width == availableResolutions[i].width &&
                Screen.currentResolution.height == availableResolutions[i].height)
                return i;
        }
        return 0;
    }
}
