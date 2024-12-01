using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    private NetworkRunner runner;

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

    void Start()
    {
        InitializeRunner();
    }

    private void InitializeRunner()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        //runner.ProvideInput = true; // Enable player input syncing
    }

    public async void StartGame()
    {
        var customProperties = new Dictionary<string, SessionProperty>()
        {
            { "RaceDistance", Player.Instance.GameSettings.RaceDistance.ToMeters() },
            { "PlayerCount", Player.Instance.GameSettings.PlayerCount.ToPlayerCount() }
        };

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "RaceGame_" + Random.Range(0, 1000), // Unique room name
            SessionProperties = customProperties,             // Set room properties
            PlayerCount = Player.Instance.GameSettings.PlayerCount.ToPlayerCount()
        });
    }
}
