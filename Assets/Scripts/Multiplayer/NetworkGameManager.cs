using Fusion;
using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    Waiting,
    Started,
    Finished
}

public class NetworkGameManager : NetworkBehaviour, IPlayerJoined
{
    [Networked, OnChangedRender(nameof(OnGameStateChanged))]
    public GameState State { get; set; }

    [Networked, Capacity(500), OnChangedRender(nameof(OnTargetStringChanged))]
    public string TargetString { get; set; }

    [Networked, OnChangedRender(nameof(OnCountDownValueChanged))]
    public int CountDownValue { get; set; } 

    private static GameState _State;

    public bool IsMaster => Runner == Runner.IsSharedModeMasterClient;

    float elapsedNetworkTime;

    //public override void FixedUpdateNetwork()
    //{
    //    base.FixedUpdateNetwork();

    //    if (IsMaster)
    //    {
    //        elapsedNetworkTime += Runner.DeltaTime;
    //        if (elapsedNetworkTime >= 1) 
    //        {
    //            elapsedNetworkTime = 0;

    //            // Stop changing the value if we've already started
    //            if (CountDownValue <= 0)
    //                return;
    //            CountDownValue--;
    //        }
    //    }
    //}

    //private void Update()
    //{
    //    if (!IsMaster)
    //    {
    //        TypingManager.Instance.targetString = TargetString;
    //    }
    //}

    public override void Spawned()
    {
        base.Spawned();

        Debug.Log("Game manager spawned");
        if (!IsMaster)
        {
            TypingManager.Instance.targetString = TargetString;
        }
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
                break;

            case GameState.Finished:
                break;
        }
    }

    private void OnTargetStringChanged()
    {
        Debug.Log("Should be setting string: " + TargetString);
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
        }
    }

    IEnumerator CountDownCoroutine()
    {
        while (CountDownValue > 0)
        {
            yield return new WaitForSeconds(1);
            CountDownValue--;
        }
    }
}
