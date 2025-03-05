using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Collections.ObjectModel;

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
        //var customProperties = new Dictionary<string, SessionProperty>()
        //{
        //    { ROOM_PROP_RACE_DISTANCE, (int)Player.Instance.GameSettings.RaceDistance },
        //    { ROOM_PROP_PLAYER_COUNT, (int)Player.Instance.GameSettings.PlayerCount },
        //    { ROOM_PROP_TYPING_DIFFICULTY, (int)Player.Instance.GameSettings.TypingDifficulty },
        //    { ROOM_PROP_AI_DIFFICULTY, (int)Player.Instance.GameSettings.AIDifficulty },
        //    { ROOM_PROP_TRACK_ENVIRONMENT, (int)Player.Instance.GameSettings.TrackEnvironment }
        //};
        //var result = await runner.StartGame(new StartGameArgs()
        //{
        //    GameMode = GameMode.Shared,
        //    SessionProperties = customProperties,             // Set room properties
        //    IsOpen = true,
        //    IsVisible = true,
        //    PlayerCount = Player.Instance.GameSettings.PlayerCount.ToInt(),
        //    Scene = NetworkScene
        //});

        var result = await runner.StartGame(GetStartGameArgs());

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

    public Dictionary<string, SessionProperty> GetDefaultSessionProperties()
    {
        var customProperties = new Dictionary<string, SessionProperty>()
        {
            { ROOM_PROP_RACE_DISTANCE, (int)RaceDistance.KM1 },
            { ROOM_PROP_PLAYER_COUNT, (int)RacePlayerCount.Eight },
            { ROOM_PROP_TYPING_DIFFICULTY, (int)TypingDifficulty.Easy },
            { ROOM_PROP_AI_DIFFICULTY, (int)AIDifficulty.Random },
            { ROOM_PROP_TRACK_ENVIRONMENT, (int)TrackEnvironment.Classic }
        };

        return customProperties;
    }

    private StartGameArgs GetStartGameArgs()
    {
        var args = new StartGameArgs();
        args.GameMode = GameMode.Shared;
        args.IsOpen = true;
        args.IsVisible = true;
        args.Scene = NetworkScene;

        // Quick play won't take any room properties into consideration just find anything
        if (Player.Instance.GameMode == RaceGameMode.QuickPlay)
            return args;

        // If its single player don't allow anyone to join
        if (Player.Instance.GameMode == RaceGameMode.SinglePlayer)
        {
            args.IsOpen = false;
            args.IsVisible = false;
        }   

        var customProperties = new Dictionary<string, SessionProperty>()
        {
            { ROOM_PROP_RACE_DISTANCE, (int)Player.Instance.GameSettings.RaceDistance },
            { ROOM_PROP_PLAYER_COUNT, (int)Player.Instance.GameSettings.PlayerCount },
            { ROOM_PROP_TYPING_DIFFICULTY, (int)Player.Instance.GameSettings.TypingDifficulty },
            { ROOM_PROP_AI_DIFFICULTY, (int)Player.Instance.GameSettings.AIDifficulty },
            { ROOM_PROP_TRACK_ENVIRONMENT, (int)Player.Instance.GameSettings.TrackEnvironment }
        };
        args.SessionProperties = customProperties;
        args.PlayerCount = Player.Instance.GameSettings.PlayerCount.ToInt();

        return args;
    }
}

