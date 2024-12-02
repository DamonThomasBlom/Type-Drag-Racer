using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    public NetworkRunner runner;
    public bool JoinOnStart = true;
    public SceneRef NetworkScene;

    public const string ROOM_PROP_RACE_DISTANCE = "RaceDistance";
    public const string ROOM_PROP_PLAYER_COUNT = "PlayerCount";
    public const string ROOM_PROP_TYPING_DIFFICULTY = "TypingDifficulty";
    public const string ROOM_PROP_AI_DIFFICULTY = "AiDifficulty";
    public const string ROOM_PROP_TRACK_ENVIRONMENT = "TrackEnvironment";

    void Awake()
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
    }

    private void Start()
    {
        if (JoinOnStart)
            StartGame();
    }

    public async void SimpleStartGame()
    {
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
        });

        if (result.Ok)
        {
            // all good
            Debug.Log("Joined room");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public async void StartGame()
    {
        var customProperties = new Dictionary<string, SessionProperty>()
        {
            { ROOM_PROP_RACE_DISTANCE, (int)Player.Instance.GameSettings.RaceDistance },
            { ROOM_PROP_PLAYER_COUNT, (int)Player.Instance.GameSettings.PlayerCount },
            { ROOM_PROP_TYPING_DIFFICULTY, (int)Player.Instance.GameSettings.TypingDifficulty },
            { ROOM_PROP_AI_DIFFICULTY, (int)Player.Instance.GameSettings.AIDifficulty },
            { ROOM_PROP_TRACK_ENVIRONMENT, (int)Player.Instance.GameSettings.TrackEnvironment }
        };
        Debug.Log("First");
        DebugSessionProperties(customProperties);
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "RaceGame_" + Random.Range(0, 1000), // Unique room name
            SessionProperties = customProperties,             // Set room properties
            IsOpen = true,
            IsVisible = true,
            PlayerCount = Player.Instance.GameSettings.PlayerCount.ToPlayerCount(),
            Scene = NetworkScene
        });

        if (result.Ok)
        {
            // all good
            Debug.Log("Joined room");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public static void DebugSessionProperties(Dictionary<string, SessionProperty> dictionary)
    {
        if (dictionary == null || dictionary.Count == 0)
        {
            Debug.Log("The dictionary is empty or null.");
            return;
        }

        foreach (var kvp in dictionary)
        {
            string key = kvp.Key;
            SessionProperty value = kvp.Value;

            // Format the key-value pair for better readability
            Debug.Log($"Key: {key}, Value: {value}");
        }
    }
}

