using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float maxShakeIntensity = 0.5f;
    public float minShakeIntensity = 0.1f;
    public float maxSpeedForMaxIntensity = 100f;
    public float minCarSpeed = 50;

    private Vector3 originalPosition;
    private ICarSpeed carSpeed;

    void Start()
    {
        originalPosition = transform.localPosition;
        carSpeed = GetComponentInParent<ICarSpeed>(); 
    }

    void Update()
    {
        if (carSpeed.speed < minCarSpeed)
        {
            return;
        }

        float speed = carSpeed.speed - minCarSpeed;
        // Calculate shake intensity based on car speed
        float shakeIntensity = Mathf.Lerp(minShakeIntensity, maxShakeIntensity, speed / maxSpeedForMaxIntensity);

        // Apply screen shake
        if (shakeIntensity > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            transform.localPosition = originalPosition + shakeOffset;
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }
}
