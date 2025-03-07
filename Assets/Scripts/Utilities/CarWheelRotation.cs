using System.Collections.Generic;
using UnityEngine;

public class CarWheelRotation : MonoBehaviour
{
    public List<Transform> LeftWheels = new List<Transform>();
    public List<Transform> RightWheels = new List<Transform>();
    public float rotationSpeedMultiplier = 40f;

    private ICarSpeed carSpeed;

    [SerializeField]
    float currentSpeed;

    private void Start()
    {
        carSpeed = GetComponentInParent<ICarSpeed>();
    }

    private void Update()
    {
        GetCurrentSpeed();

        float rotationAmount = currentSpeed * rotationSpeedMultiplier * Time.deltaTime;

        foreach (Transform wheel in LeftWheels)
            wheel.Rotate(Vector3.right, rotationAmount);

        foreach (Transform wheel in RightWheels)
            wheel.Rotate(Vector3.right, rotationAmount);
    }

    void GetCurrentSpeed()
    {
        if (carSpeed != null)
            currentSpeed = carSpeed.speed;
    }
}
