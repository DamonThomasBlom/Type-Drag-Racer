using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RemotePlayer : NetworkBehaviour, ICarSpeed
{
    public GameStatistics stats;
    public float speed { get; set; }

    [Networked, OnChangedRender(nameof(SpawnCar))]
    public string carPrefabName { get; set; }

    [Networked, OnChangedRender(nameof(SetPlayerName))]
    public string playerName { get; set; }

    public bool isLocalPlayer => Object && Object.HasStateAuthority;

    public Transform carSpawnPoint;

    public TextMeshProUGUI nameText;

    private Transform localPlayerTransform;
    private PlayerController localPlayerController;
    private GameObject carInstance;
    private AudioSource carAudio;

    public override void Spawned()
    {
        base.Spawned();
        if (isLocalPlayer)
        {
            playerName = Player.Instance.username;
            carPrefabName = Player.Instance.carName;
            localPlayerTransform = Player.Instance.playerTransform;
            localPlayerController = localPlayerTransform.GetComponent<PlayerController>();
            carSpawnPoint.gameObject.SetActive(false);
            carAudio = GetComponent<AudioSource>();

            carAudio.enabled = false;
            if (nameText != null)
                nameText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            SpawnCar();
            SetPlayerName();
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (isLocalPlayer && localPlayerTransform)
        {
            ApplyLocalData();
        }
    }

    private void SpawnCar()
    {
        if (string.IsNullOrEmpty(carPrefabName)) { return; }
        if (carInstance != null) { return; }

        var selectedCarPrefab = PrefabManager.Instance.GetCarPrefab(carPrefabName);
        carInstance = Instantiate(selectedCarPrefab, carSpawnPoint.position, carSpawnPoint.rotation, carSpawnPoint);
    }

    private void SetPlayerName()
    {
        if (nameText != null)
            nameText.text = playerName;
    }

    protected virtual void ApplyLocalData()
    {
        transform.position = localPlayerTransform.position;
        stats = localPlayerController.gameStatistics;
        speed = localPlayerController.speed;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void FinishedRaceRpc(float time, string name, int wordsPerMinute)
    {
        RaceLeaderboardManager.Instance.AddEntry(new LeaderboardEntry() { FinishTime = time, Name = name, WPM = wordsPerMinute });
    }
}
