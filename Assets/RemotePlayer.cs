using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RemotePlayer : NetworkBehaviour
{
    public GameStatistics stats;
    public float currentSpeed;

    public bool isLocalPlayer => Object && Object.HasStateAuthority;

    public GameObject GFX;

    private Transform localPlayerTransform;
    private PlayerController localPlayerController;

    public override void Spawned()
    {
        base.Spawned();
        if (isLocalPlayer)
        {
            localPlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            localPlayerController = localPlayerTransform.GetComponent<PlayerController>();
            GFX.SetActive(false);
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

    protected virtual void ApplyLocalData()
    {
        transform.position = localPlayerTransform.position;
        stats = localPlayerController.gameStatistics;
        currentSpeed = localPlayerController.currentSpeed;
    }
}
