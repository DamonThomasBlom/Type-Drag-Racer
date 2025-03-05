using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;

public static class GameSettingsExtensions
{
    // PlayerCount Extensions
    public static string GetDescription(this RacePlayerCount playerCount)
    {
        return $"{(int)playerCount} Players";
    }

    public static int ToInt(this RacePlayerCount playerCount)
    {
        switch (playerCount)
        {
            case RacePlayerCount.Two: return 2;
            case RacePlayerCount.Four: return 4;
            case RacePlayerCount.Six: return 6;
            case RacePlayerCount.Eight: return 8;
            default: return 8;
        }
    }

    public static string ToPlayerString(this RacePlayerCount playerCount)
    {
        switch (playerCount)
        {
            case RacePlayerCount.Two: return "Two";
            case RacePlayerCount.Four: return "Four";
            case RacePlayerCount.Six: return "Six";
            case RacePlayerCount.Eight: return "Eight";
        }

        return string.Empty;
    }

    // RaceDistance Extensions
    public static string GetDescription(this RaceDistance raceDistance)
    {
        switch (raceDistance)
        {
            case RaceDistance.M500: return "500m";
            case RaceDistance.KM1: return "1 km";
            case RaceDistance.KM2: return "2 km";
            case RaceDistance.KM5: return "5 km";
            case RaceDistance.KM10: return "10 km";
            default: return "Unknown Distance";
        }
    }

    public static int ToMeters(this RaceDistance raceDistance)
    {
        switch (raceDistance)
        {
            case RaceDistance.M500: return 500;
            case RaceDistance.KM1: return 1000;
            case RaceDistance.KM2: return 2000;
            case RaceDistance.KM5: return 5000;
            case RaceDistance.KM10: return 10000;
            default: return 500;
        }
    }

    // TypingDifficulty Extensions
    public static string GetDescription(this TypingDifficulty difficulty)
    {
        switch (difficulty)
        {
            case TypingDifficulty.Easy: return "Easy";
            case TypingDifficulty.Medium: return "Medium";
            case TypingDifficulty.Hard: return "Hard";
            default: return "Unknown Difficulty";
        }
    }

    // AIDifficulty Extensions
    public static string GetDescription(this AIDifficulty difficulty)
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy: return "Easy AI";
            case AIDifficulty.Medium: return "Medium AI";
            case AIDifficulty.Hard: return "Hard AI";
            case AIDifficulty.Expert: return "Expert AI";
            case AIDifficulty.Random: return "Random AI Difficulty";
            default: return "Unknown AI Difficulty";
        }
    }

    public static List<float> ToBotSpawnChances(this AIDifficulty aIDifficulty)
    {
        List<float> probablities = new List<float>();

        switch (aIDifficulty)
        {
            case AIDifficulty.Easy:
                probablities = new List<float> { 0.4f, 0.6f, 0f, 0, 0, 0 };
                break;

            case AIDifficulty.Medium:
                probablities = new List<float> { 0, 0.3f, 0.6f, 0.1f, 0, 0 };
                break;

            case AIDifficulty.Hard:
                probablities = new List<float> { 0, 0, 0.1f, 0.6f, 0.3f, 0f };
                break;

            case AIDifficulty.Expert:
                probablities = new List<float> { 0f, 0f, 0f, 0.1f, 0.2f, 0.7f };
                break;

            case AIDifficulty.Random:
                probablities = new List<float> { 0.4f, 0.3f, 0.11f, 0.09f, 0.07f, 0.03f };
                break;
        }

        return probablities;
    }

    public static AIDifficulty GetRandomDifficulty()
    {
        var difficulties = new[] { AIDifficulty.Easy, AIDifficulty.Medium, AIDifficulty.Hard, AIDifficulty.Expert };
        return difficulties[UnityEngine.Random.Range(0, difficulties.Length)];
    }

    // TrackEnvironment Extensions
    public static string GetDescription(this TrackEnvironment environment)
    {
        switch (environment)
        {
            case TrackEnvironment.Classic: return "Classic Track";
            case TrackEnvironment.Desert: return "Desert Track";
            case TrackEnvironment.Forest: return "Forest Track";
            case TrackEnvironment.Urban: return "Urban Track";
            default: return "Unknown Environment";
        }
    }
}
