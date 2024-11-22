using Fusion;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

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

    public override void Spawned()
    {
        base.Spawned();
        joined = true;

        if (CurrentStatusText != null)
            CurrentStatusText.text = "Waiting for players...";

        OnRaceDistanceChanged();

        if (!IsMaster)
        {
            TypingManager.Instance.targetString = TargetString;
        }
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
                TypingManager.Instance.gameStarted = true;
                RaceStartTimeNetwork = ElapsedNetworkTime;
                break;

            case GameState.Finished:
                break;
        }
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
            SetState(GameState.Started);

        if (CountdownUI.Instance != null)
        {
            CountdownUI.Instance.CountDownValue(CountDownValue);
        }
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
