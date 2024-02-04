using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheelRotation : MonoBehaviour
{
    public Transform[] wheels;
    public float rotationSpeedMultiplier = 40f;

    private PlayerController playerController;
    private RemotePlayer remotePlayer;
    private AIPlayer botPlayer;

    [SerializeField]
    float currentSpeed;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        remotePlayer = GetComponentInParent<RemotePlayer>();
        botPlayer = GetComponentInParent<AIPlayer>();
    }

    private void Update()
    {
        GetCurrentSpeed();

        float rotationAmount = currentSpeed * rotationSpeedMultiplier * Time.deltaTime;

        foreach (Transform wheel in wheels)
            wheel.Rotate(Vector3.right, rotationAmount);
    }

    void GetCurrentSpeed()
    {
        if (playerController)
            currentSpeed = playerController.currentSpeed;

        if (remotePlayer)
            currentSpeed = remotePlayer.currentSpeed;

        if (botPlayer)
            currentSpeed = botPlayer.currentSpeed;
    }
}
