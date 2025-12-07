using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedVirtualCamera : MonoBehaviour
{
    public Vector3 targetPosition;

    public float MaxCarSpeed;
    [Tooltip("Speed at which FOV starts increasing")]
    public float MinFOVSpeed = 0f;

    [Tooltip("Speed at which camera offset starts moving")]
    public float MinOffsetSpeed = 0f;

    [Tooltip("Speed at which camera shake begins")]
    public float MinShakeSpeed = 0f;

    private Vector3 _startPosition;

    private const float FIELD_OF_VIEW_MIN = 50f;
    private const float FIELD_OF_VIEW_MAX = 100f;
    private const float NOISE_AMPLITUDE_MAX = 1.4f;
    private const float NOISE_FREQUENCY_MAX = 1.4f;

    [SerializeField] private ICarSpeed _carSpeed;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer cinemachineTransposer;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        _carSpeed = GetComponentInParent<ICarSpeed>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _startPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        float speed = _carSpeed.speed;

        // --- FOV ---
        LensSettings lensSettings = virtualCamera.m_Lens;
        lensSettings.FieldOfView = Mathf.Lerp(
            FIELD_OF_VIEW_MIN,
            FIELD_OF_VIEW_MAX,
            NormalizeSpeed(speed, MinFOVSpeed, MaxCarSpeed)
        );
        virtualCamera.m_Lens = lensSettings;

        // --- Camera Offset ---
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(
            _startPosition,
            targetPosition,
            NormalizeSpeed(speed, MinOffsetSpeed, MaxCarSpeed)
        );

        // --- Camera Shake ---
        float shakeT = NormalizeSpeed(speed, MinShakeSpeed, MaxCarSpeed);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(0f, NOISE_AMPLITUDE_MAX, shakeT);
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = Mathf.Lerp(0f, NOISE_FREQUENCY_MAX, shakeT);
    }

    float NormalizeSpeed(float speed, float minSpeed, float maxSpeed)
    {
        if (speed <= minSpeed) return 0f;
        return Mathf.Clamp01((speed - minSpeed) / (maxSpeed - minSpeed));
    }
}
