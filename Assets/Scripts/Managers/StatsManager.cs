using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class PlayerStats
{
    public const string TOTAL_RACES_COMPLETED = "Total Races Completed";
    public const string TOTAL_WINS = "Total Wins";
    public const string TOTAL_LOSSES = "Total Losses";
    public const string BEST_WPM = "Best WPM";
    public const string AVERAGE_WPM = "Average WPM";
    public const string BEST_ACCURACY = "Best Accuracy";
    public const string AVERAGE_ACCURACY = "Average Accuracy";
    public const string FASTEST_RACE_TIME = "Fastest Race Time";
    public const string AVERAGE_RACE_TIME = "Average Race Time";
    public const string TOTAL_DISTANCE_RACED = "Total Distance Raced";
    public const string TOTAL_NITRO_USED = "Total NitroUsed";
    public const string TOTAL_KEYS_PRESSED = "Total Keys Pressed";
    public const string TOTAL_MISTAKES_MADE = "Total Mistakes Made";
    public const string TOTAL_PERFECT_RACES = "Total Perfect Races";
    public const string TOTAL_TIME_PLAYED = "Total Time Played";
    public const string MOST_USED_RACE_DISTANCE = "Most Used Race Distance";
    public const string LONGEST_TYPING_STREAK = "Longest Typing Streak";
    public const string HIGHEST_COMBO_STREAK = "Highest Combo Streak";
    public const string TOTAL_WORDS_TYPED = "Total Words Typed";
    public const string TOTAL_CHARACTERS_TYPED = "Total Characters Typed";

    [ShowInInspector] public Dictionary<string, string> Stats = new Dictionary<string, string>();

    [ShowInInspector] public Queue<int> Last10RacesWPM = new Queue<int>();

    public PlayerStats()
    {
        Stats = new Dictionary<string, string>
        {
            { TOTAL_RACES_COMPLETED, "0" },
            { TOTAL_WINS, "0" },
            { TOTAL_LOSSES, "0" },
            { BEST_WPM, "0" },
            { AVERAGE_WPM, "0" },
            { BEST_ACCURACY, "0" },
            { AVERAGE_ACCURACY, "0" },
            { FASTEST_RACE_TIME, "0" },
            { AVERAGE_RACE_TIME, "0" },
            { TOTAL_DISTANCE_RACED, "0" },
            { TOTAL_NITRO_USED, "0" },
            { TOTAL_KEYS_PRESSED, "0" },
            { TOTAL_MISTAKES_MADE, "0" },
            { TOTAL_PERFECT_RACES, "0" },
            { TOTAL_TIME_PLAYED, "0" },
            { MOST_USED_RACE_DISTANCE, "None" },
            { LONGEST_TYPING_STREAK, "0" },
            { HIGHEST_COMBO_STREAK, "0" },
            { TOTAL_WORDS_TYPED, "0" },
            { TOTAL_CHARACTERS_TYPED, "0" }
        };
    }
}

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    private string savePath;
    [ShowInInspector] public PlayerStats playerStats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Path.Combine(Application.persistentDataPath, "playerStats.json");
        LoadStats();
    }

    [Button]
    public void SaveStats()
    {
        string json = JsonConvert.SerializeObject(playerStats);
        File.WriteAllText(savePath, json);
        Debug.Log("Stats Saved: " + savePath);
    }

    public void LoadStats()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            playerStats = JsonConvert.DeserializeObject<PlayerStats>(json);
            Debug.Log("Stats Loaded");
        }
        else
        {
            playerStats = new PlayerStats();
        }
    }

    [Button]
    public void ResetStats()
    {
        playerStats = new PlayerStats();
        SaveStats();
    }

    public void UpdateStat(string statName, string value)
    {
        if (playerStats.Stats.ContainsKey(statName))
        {
            playerStats.Stats[statName] = value;
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' does not exist.");
        }
        SaveStats();
    }

    public void IncreaseStatByValue(string statName, float value)
    {
        if (playerStats.Stats.ContainsKey(statName))
        {
            playerStats.Stats[statName] = (float.Parse(playerStats.Stats[statName]) + value).ToString();
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' does not exist.");
        }
        SaveStats();
    }

    public void UpdateStatIfHigher(string statName, float value)
    {
        if (playerStats.Stats.ContainsKey(statName))
        {
            if (float.Parse(playerStats.Stats[statName]) < value)
                playerStats.Stats[statName] = value.ToString();
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' does not exist.");
        }
        SaveStats();
    }

    public void UpdateStatIfLower(string statName, float value)
    {
        if (playerStats.Stats.ContainsKey(statName))
        {
            float currentValue = float.Parse(playerStats.Stats[statName]);
            if (currentValue == 0 || currentValue > value)
                playerStats.Stats[statName] = value.ToString();
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' does not exist.");
        }
        SaveStats();
    }

    public void UpdateAverage(string statName, float incomingValue)
    {
        if (playerStats.Stats.ContainsKey(statName))
        {
            // Calcualte average
            float currentAverage = float.Parse(playerStats.Stats[statName]);
            float totalRaces = float.Parse(playerStats.Stats[PlayerStats.TOTAL_RACES_COMPLETED]);
            float total = (currentAverage * (totalRaces - 1)) + incomingValue;

            playerStats.Stats[statName] = (total / totalRaces).ToString();
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' does not exist.");
        }
        SaveStats();
    }

    public void AddToLast10Races(int wpm)
    {
        if (playerStats.Last10RacesWPM.Count >= 10)
            playerStats.Last10RacesWPM.Dequeue();

        playerStats.Last10RacesWPM.Enqueue(wpm);
        SaveStats();
    }

    public string GetStat(string statName)
    {
        if (playerStats.Stats.ContainsKey(statName))
        {
            return playerStats.Stats[statName];
        }
        Debug.LogWarning("Stat not found: " + statName);
        return "0";
    }

}
