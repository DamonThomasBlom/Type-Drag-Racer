using Michsky.MUIP;
using System;
using UnityEngine;

// ENUMS
public enum RacePlayerCount { Two, Four, Six, Eight }
public enum RaceDistance { M500, KM1, KM2, KM5, KM10 }
public enum TypingDifficulty { Easy, Medium, Hard }
public enum AIDifficulty { Easy, Medium, Hard, Expert,  Random }
public enum TrackEnvironment { Classic, Desert, Forest, Urban }

public class GameSettingsUI : MonoBehaviour
{
    [Header("MUIP Dropdowns")]
    public CustomDropdown playersDropdown;
    public CustomDropdown raceDistanceDropdown;
    public CustomDropdown typingDifficultyDropdown;
    public CustomDropdown aiDifficultyDropdown;
    public CustomDropdown trackEnvironmentDropdown;

    [Header("Defaults")]
    private readonly RacePlayerCount defaultPlayers = RacePlayerCount.Eight;
    private readonly RaceDistance defaultRaceDistance = RaceDistance.M500;
    private readonly TypingDifficulty defaultTypingDifficulty = TypingDifficulty.Medium;
    private readonly AIDifficulty defaultAIDifficulty = AIDifficulty.Random;
    private readonly TrackEnvironment defaultTrackEnvironment = TrackEnvironment.Classic;

    private void Start()
    {
        InitializeDropdown(playersDropdown, typeof(RacePlayerCount));
        InitializeDropdown(raceDistanceDropdown, typeof(RaceDistance));
        InitializeDropdown(typingDifficultyDropdown, typeof(TypingDifficulty));
        InitializeDropdown(aiDifficultyDropdown, typeof(AIDifficulty));
        InitializeDropdown(trackEnvironmentDropdown, typeof(TrackEnvironment));

        LoadSettings();
        AddDropdownListeners();

        typingDifficultyDropdown.isInteractable = false;
        trackEnvironmentDropdown.isInteractable = false;
    }

    private void InitializeDropdown(CustomDropdown dropdown, Type enumType)
    {
        dropdown.items.Clear();

        foreach (string name in Enum.GetNames(enumType))
        {
            var item = new CustomDropdown.Item
            {
                itemName = name,
                OnItemSelection = new UnityEngine.Events.UnityEvent()
            };

            dropdown.items.Add(item);
        }

        dropdown.SetupDropdown();
    }

    private void LoadSettings()
    {
        playersDropdown.SetDropdownIndex(PlayerPrefs.GetInt("NumberOfPlayers", (int)defaultPlayers));
        raceDistanceDropdown.SetDropdownIndex(PlayerPrefs.GetInt("RaceDistance", (int)defaultRaceDistance));
        typingDifficultyDropdown.SetDropdownIndex(PlayerPrefs.GetInt("TypingDifficulty", (int)defaultTypingDifficulty));
        aiDifficultyDropdown.SetDropdownIndex(PlayerPrefs.GetInt("AIDifficulty", (int)defaultAIDifficulty));
        trackEnvironmentDropdown.SetDropdownIndex(PlayerPrefs.GetInt("TrackEnvironment", (int)defaultTrackEnvironment));

        ApplySettings();
    }

    private void AddDropdownListeners()
    {
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
        GameSettings gameSettings = new GameSettings
        {
            PlayerCount = (RacePlayerCount)playersDropdown.selectedItemIndex,
            RaceDistance = (RaceDistance)raceDistanceDropdown.selectedItemIndex,
            TypingDifficulty = (TypingDifficulty)typingDifficultyDropdown.selectedItemIndex,
            AIDifficulty = (AIDifficulty)aiDifficultyDropdown.selectedItemIndex,
            TrackEnvironment = (TrackEnvironment)trackEnvironmentDropdown.selectedItemIndex
        };

        Player.Instance.GameSettings = gameSettings;

        Debug.Log($"Settings Applied:\n" +
          $"Players: {gameSettings.PlayerCount}\n" +
          $"Race Distance: {gameSettings.RaceDistance}\n" +
          $"Typing Difficulty: {gameSettings.TypingDifficulty}\n" +
          $"AI Difficulty: {gameSettings.AIDifficulty}\n" +
          $"Track Environment: {gameSettings.TrackEnvironment}");
    }
}
