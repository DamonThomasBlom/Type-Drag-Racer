using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RemotePlayer : NetworkBehaviour, ICarSpeed
{
    public GameStatistics stats;
    public float speed { get; set; }

    [Networked, OnChangedRender(nameof(SpawnCar))]
    public string carPrefabName { get; set; }

    public bool isLocalPlayer => Object && Object.HasStateAuthority;

    public Transform carSpawnPoint;

    private Transform localPlayerTransform;
    private PlayerController localPlayerController;
    private GameObject carInstance;
    private AudioSource carAudio;

    public override void Spawned()
    {
        base.Spawned();
        if (isLocalPlayer)
        {
            carPrefabName = Player.Instance.carName;
            localPlayerTransform = Player.Instance.playerTransform;
            localPlayerController = localPlayerTransform.GetComponent<PlayerController>();
            carSpawnPoint.gameObject.SetActive(false);
            carAudio = GetComponent<AudioSource>();

            carAudio.enabled = false;
        }
        else
        {
            SpawnCar();
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

    //public override void Render()
    //{
    //    base.Render();
    //    if (isLocalPlayer)
    //    {
    //        // Extrapolate for local user :
    //        // we want to have the visual at the good position as soon as possible, so we force the visuals to follow the most fresh hardware positions
    //        transform.position = localPlayerTransform.position;
    //        stats = localPlayerController.gameStatistics;
    //        currentSpeed = localPlayerController.currentSpeed;
    //    }
    //}

    private void SpawnCar()
    {
        if (string.IsNullOrEmpty(carPrefabName)) { return; }
        if (carInstance != null) { return; }

        var selectedCarPrefab = PrefabManager.Instance.GetCarPrefab(carPrefabName);
        carInstance = Instantiate(selectedCarPrefab, carSpawnPoint.position, carSpawnPoint.rotation, carSpawnPoint);
    }

    protected virtual void ApplyLocalData()
    {
        transform.position = localPlayerTransform.position;
        stats = localPlayerController.gameStatistics;
        speed = localPlayerController.speed;
    }
}
