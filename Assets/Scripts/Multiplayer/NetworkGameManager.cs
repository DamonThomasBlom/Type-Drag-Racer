using Fusion;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Waiting,
    Started,
    Finished
}

public class NetworkGameManager : NetworkBehaviour, IPlayerJoined
{
    public static NetworkGameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Networked, OnChangedRender(nameof(OnGameStateChanged))]
    public GameState State { get; set; }

    [Networked, Capacity(500), OnChangedRender(nameof(OnTargetStringChanged))]
    public string TargetString { get; set; }

    [Networked, OnChangedRender(nameof(OnCountDownValueChanged))]
    public int CountDownValue { get; set; }

    [Networked, OnChangedRender(nameof(OnRaceDistanceChanged))] 
    public float RaceDistance { get; set; }

    private static GameState _State;

    public bool IsMaster => Runner == Runner.IsSharedModeMasterClient;

    public TextMeshProUGUI CurrentStatusText;

    [Networked]
    public float ElapsedNetworkTime {  get; set; }

    [Networked]
    public float RaceStartTimeNetwork { get; set; }

    // UNITY EVENTS
    public UnityEvent OnGameStarted = new UnityEvent();
    public UnityEvent OnGameFinished = new UnityEvent();

    public float MasterClientPing;

    public override void Spawned()
    {
        base.Spawned();
        joined = true;
        InitializeGameSettings();

        DelayedActionUtility.Instance.PerformActionWithDelay(3f, () =>
        {
            if (CurrentStatusText != null)
                CurrentStatusText.text = "Waiting for players...";
        });

        OnRaceDistanceChanged();

        if (!IsMaster)
        {
            TypingManager.Instance.targetString = TargetString;
        }
    }

    void InitializeGameSettings()
    {
        var RoomProperties = Runner.SessionInfo.Properties;
        RaceDistance distanceEnum = (RaceDistance)(int)RoomProperties[NetworkManager.ROOM_PROP_RACE_DISTANCE];
        RaceDistance = distanceEnum.ToMeters();

        Debug.Log("Second");
        //NetworkManager.DebugSessionProperties(RoomProperties);

        Debug.Log($"Ints {(int)RoomProperties[NetworkManager.ROOM_PROP_PLAYER_COUNT]} - {(int)RoomProperties[NetworkManager.ROOM_PROP_RACE_DISTANCE]}" +
            $" - {(int)RoomProperties[NetworkManager.ROOM_PROP_TYPING_DIFFICULTY]} - {(int)RoomProperties[NetworkManager.ROOM_PROP_AI_DIFFICULTY]}" +
            $" - {(int)RoomProperties[NetworkManager.ROOM_PROP_TRACK_ENVIRONMENT]}");

        GameManager.Instance.PlayerCount = (RacePlayerCount)(int)RoomProperties[NetworkManager.ROOM_PROP_PLAYER_COUNT];
        GameManager.Instance.RaceDistanceEnum = (RaceDistance)(int)RoomProperties[NetworkManager.ROOM_PROP_RACE_DISTANCE];
        GameManager.Instance.TypingDifficulty = (TypingDifficulty)(int)RoomProperties[NetworkManager.ROOM_PROP_TYPING_DIFFICULTY];
        GameManager.Instance.AIDifficulty = (AIDifficulty)(int)RoomProperties[NetworkManager.ROOM_PROP_AI_DIFFICULTY];
        GameManager.Instance.TrackEnvironment = (TrackEnvironment)(int)RoomProperties[NetworkManager.ROOM_PROP_TRACK_ENVIRONMENT];
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        ElapsedNetworkTime += Runner.DeltaTime;
    }

    private void Update()
    {
        if (joined && IsMaster && !countDownStarted)
            StartCoroutine(CountDownCoroutine());
        if (joined && IsMaster)
            MasterClientPing = GetPing();
    }

    private void OnRaceDistanceChanged()
    {
        GameManager.Instance.RaceDistance = RaceDistance;
        FindObjectOfType<WorldGenerator>().SetupFinishLine(RaceDistance);
    }

    private void OnGameStateChanged()
    {
        _State = State;
        switch (State)
        {
            case GameState.Waiting:
                break;

            case GameState.Started:
                // Because the master is controlling the flow of the game they have slight advantage because of their ping, this resolves 
                // the issue of the master starting the game with an advantage
                if (IsMaster)
                    DelayedActionUtility.Instance.PerformActionWithDelay(MasterClientPing, () => StartRace());
                else
                    StartRace();
                break;

            case GameState.Finished:
                OnGameFinished.Invoke();
                break;
        }
    }

    private void StartRace()
    {
        TypingManager.Instance.gameStarted = true;
        RaceStartTimeNetwork = ElapsedNetworkTime;
        OnGameStarted.Invoke();
    }

    private void OnTargetStringChanged()
    {
        TypingManager.Instance.targetString = TargetString;
    }

    private void OnCountDownValueChanged()
    {
        Debug.Log("Countdown value changed: " + CountDownValue);
        if (IsMaster && CountDownValue == 5)
        {
            // Generate bots to full any empty spaces and disable other players joining last second
            SpawnpointManager.Instance.GenerateBots();
            Runner.SessionInfo.IsOpen = false;
        }

        if (CountDownValue == 5)
            if (CurrentStatusText != null)
                CurrentStatusText.gameObject.SetActive(false);

        if (IsMaster && CountDownValue <= 0)
        {
            SetState(GameState.Started);
        }

        if (CountdownUI.Instance != null)
        {
            CountdownUI.Instance.CountDownValue(CountDownValue);
        }
    }

    public float GetPing()
    {
        float ping = (float)Runner.GetPlayerRtt(Runner.LocalPlayer);
        //Debug.Log("Ping: " + ping.ToString("F2"));

        return ping;
    }

    public void SetTargetString()
    {
        TargetString = TypingManager.Instance.targetString;
    }

    public void SetState(GameState newState)
    {
        State = newState;
        _State = newState;
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (IsMaster)
        {
            Invoke(nameof(SetTargetString), 2);

            // Only start the countdown if we are the player
            if (player == Runner.LocalPlayer)
            {
                CountDownValue = 15;
                StartCoroutine(CountDownCoroutine());
            }
        }
        else
        {
            OnTargetStringChanged();
            OnCountDownValueChanged();
            OnGameStateChanged();
            OnRaceDistanceChanged();
        }
    }

    bool countDownStarted;
    bool joined;

    IEnumerator CountDownCoroutine()
    {
        countDownStarted = true;

        while (CountDownValue > 0)
        {
            yield return new WaitForSeconds(1);
            CountDownValue--;
        }
    }
}
