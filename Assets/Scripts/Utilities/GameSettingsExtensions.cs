using System;
using UnityEngine;

public static class GameSettingsExtensions
{
    // PlayerCount Extensions
    public static string GetDescription(this PlayerCount playerCount)
    {
        return $"{(int)playerCount} Players";
    }

    public static int ToPlayerCount(this PlayerCount playerCount)
    {
        switch (playerCount)
        {
            case PlayerCount.Two: return 2;
            case PlayerCount.Four: return 4;
            case PlayerCount.Six: return 6;
            case PlayerCount.Eight: return 8;
            default: return 8;
        }
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
        return (int)raceDistance;
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
            case AIDifficulty.Random: return "Random AI Difficulty";
            default: return "Unknown AI Difficulty";
        }
    }

    public static AIDifficulty GetRandomDifficulty()
    {
        var difficulties = new[] { AIDifficulty.Easy, AIDifficulty.Medium, AIDifficulty.Hard };
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
