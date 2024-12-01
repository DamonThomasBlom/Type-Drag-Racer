using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ENUMS
public enum PlayerCount { Two = 2, Four = 4, Six = 6, Eight = 8 }
public enum RaceDistance { M500 = 500, KM1 = 1000, KM2 = 2000, KM5 = 5000, KM10 = 10000 }
public enum TypingDifficulty { Easy, Medium, Hard }
public enum AIDifficulty { Easy, Medium, Hard, Random }
public enum TrackEnvironment { Classic, Desert, Forest, Urban }

public class GameSettingsUI : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown playersDropdown;
    public TMP_Dropdown raceDistanceDropdown;
    public TMP_Dropdown typingDifficultyDropdown;
    public TMP_Dropdown aiDifficultyDropdown;
    public TMP_Dropdown trackEnvironmentDropdown;

    [Header("Defaults")]
    private readonly PlayerCount defaultPlayers = PlayerCount.Eight;
    private readonly RaceDistance defaultRaceDistance = RaceDistance.M500;
    private readonly TypingDifficulty defaultTypingDifficulty = TypingDifficulty.Medium;
    private readonly AIDifficulty defaultAIDifficulty = AIDifficulty.Random;
    private readonly TrackEnvironment defaultTrackEnvironment = TrackEnvironment.Classic;

    private void Start()
    {
        InitializeDropdowns();
        LoadSettings();
        AddDropdownListeners();

        typingDifficultyDropdown.interactable = false;
        aiDifficultyDropdown.interactable = false;
        trackEnvironmentDropdown.interactable = false;
    }

    private void InitializeDropdowns()
    {
        // Players dropdown
        playersDropdown.ClearOptions();
        playersDropdown.AddOptions(
            new List<TMP_Dropdown.OptionData>(
                Enum.GetNames(typeof(PlayerCount))
                .Select(name => new TMP_Dropdown.OptionData(name))
                .ToList()
            )
        );

        // Race distance dropdown
        raceDistanceDropdown.ClearOptions();
        raceDistanceDropdown.AddOptions(
            new List<TMP_Dropdown.OptionData>(
                Enum.GetNames(typeof(RaceDistance))
                .Select(name => new TMP_Dropdown.OptionData(name))
                .ToList()
            )
        );

        // Typing difficulty dropdown
        typingDifficultyDropdown.ClearOptions();
        typingDifficultyDropdown.AddOptions(
            new List<TMP_Dropdown.OptionData>(
                Enum.GetNames(typeof(TypingDifficulty))
                .Select(name => new TMP_Dropdown.OptionData(name))
                .ToList()
            )
        );

        // AI difficulty dropdown
        aiDifficultyDropdown.ClearOptions();
        aiDifficultyDropdown.AddOptions(
            new List<TMP_Dropdown.OptionData>(
                Enum.GetNames(typeof(AIDifficulty))
                .Select(name => new TMP_Dropdown.OptionData(name))
                .ToList()
            )
        );

        // Track environment dropdown
        trackEnvironmentDropdown.ClearOptions();
        trackEnvironmentDropdown.AddOptions(
            new List<TMP_Dropdown.OptionData>(
                Enum.GetNames(typeof(TrackEnvironment))
                .Select(name => new TMP_Dropdown.OptionData(name))
                .ToList()
            )
        );
    }

    private void LoadSettings()
    {
        // Load or set defaults if PlayerPrefs are missing
        playersDropdown.value = PlayerPrefs.GetInt("NumberOfPlayers", (int)defaultPlayers);
        raceDistanceDropdown.value = PlayerPrefs.GetInt("RaceDistance", (int)defaultRaceDistance);
        typingDifficultyDropdown.value = PlayerPrefs.GetInt("TypingDifficulty", (int)defaultTypingDifficulty);
        aiDifficultyDropdown.value = PlayerPrefs.GetInt("AIDifficulty", (int)defaultAIDifficulty);
        trackEnvironmentDropdown.value = PlayerPrefs.GetInt("TrackEnvironment", (int)defaultTrackEnvironment);

        ApplySettings();
    }

    private void AddDropdownListeners()
    {
        // Add listeners for dropdowns and toggle
        playersDropdown.onValueChanged.AddListener((val) => SaveSetting("NumberOfPlayers", val));
        raceDistanceDropdown.onValueChanged.AddListener((val) => SaveSetting("RaceDistance", val));
        typingDifficultyDropdown.onValueChanged.AddListener((val) => SaveSetting("TypingDifficulty", val));
        aiDifficultyDropdown.onValueChanged.AddListener((val) => SaveSetting("AIDifficulty", val));
        trackEnvironmentDropdown.onValueChanged.AddListener((val) => SaveSetting("TrackEnvironment", val));
    }

    private void SaveSetting(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();

        ApplySettings();
    }

    public void ApplySettings()
    {
        // Apply all settings, e.g., in the game manager
        PlayerCount playerCount = (PlayerCount)playersDropdown.value;
        RaceDistance raceDistance = (RaceDistance)raceDistanceDropdown.value;
        TypingDifficulty typingDifficulty = (TypingDifficulty)typingDifficultyDropdown.value;
        AIDifficulty aiDifficulty = (AIDifficulty)aiDifficultyDropdown.value;
        TrackEnvironment trackEnvironment = (TrackEnvironment)trackEnvironmentDropdown.value;

        GameSettings gameSettings = new GameSettings();
        gameSettings.PlayerCount = playerCount;
        gameSettings.RaceDistance = raceDistance;
        gameSettings.TypingDifficulty = typingDifficulty;
        gameSettings.AIDifficulty = aiDifficulty;
        gameSettings.TrackEnvironment = trackEnvironment;

        Player.Instance.GameSettings = gameSettings;

        Debug.Log($"Settings Applied:\n" +
          $"Players: {playerCount.ToString()}\n" +
          $"Race Distance: {raceDistance.ToString()}\n" +
          $"Typing Difficulty: {typingDifficulty.ToString()}\n" +
          $"AI Difficulty: {aiDifficulty.ToString()}\n" +
          $"Track Environment: {trackEnvironment.ToString()}");
    }
}
