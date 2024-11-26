using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public float targetFOV = 60f;
    public float maxCarSpeed = 300;
    public float maxShakeIntensity = 0.5f;
    public float minShakeIntensity = 0.1f;
    public float maxSpeedForMaxIntensity = 100f;
    public float minCarSpeed = 50;

    private Camera _cam;
    private ICarSpeed _carSpeed;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float _startCamFOV;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _carSpeed = GetComponentInParent<ICarSpeed>();
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _startCamFOV = _cam.fieldOfView;
    }

    private void Update()
    {
        LerpCamera(_carSpeed.speed);
    }

    public void LerpCamera(float speed)
    {
        float lerpValue = Mathf.Clamp01(speed / maxCarSpeed);

        // Lerp camera position
        transform.localPosition = Vector3.Lerp(_startPosition, targetPosition, lerpValue);

        // Lerp camera rotation
        transform.localRotation = Quaternion.Lerp(_startRotation, targetRotation, lerpValue);

        // Lerp camera field of view (POV)
        _cam.fieldOfView = Mathf.Lerp(_startCamFOV, targetFOV, lerpValue);

        if (speed < minCarSpeed)
        {
            return;
        }

        float newSpeed = speed - minCarSpeed;
        // Calculate shake intensity based on car speed
        float shakeIntensity = Mathf.Lerp(minShakeIntensity, maxShakeIntensity, newSpeed / maxSpeedForMaxIntensity);
        Vector3 currentPostion = transform.localPosition;

        // Apply screen shake
        if (shakeIntensity > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            transform.localPosition = currentPostion + shakeOffset;
        }
    }
}
