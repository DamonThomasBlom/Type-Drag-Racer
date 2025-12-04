using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedVirtualCamera : MonoBehaviour
{
    public Vector3 targetPosition;
    public float MaxCarSpeed;
    private Vector3 _startPosition;
    private const float FIELD_OF_VIEW_MIN = 40f;
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
        LensSettings lensSettings = virtualCamera.m_Lens;
        lensSettings.FieldOfView = Mathf.Lerp(FIELD_OF_VIEW_MIN, FIELD_OF_VIEW_MAX, GetSpeedNormalized());
        virtualCamera.m_Lens = lensSettings;

        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(_startPosition, targetPosition, GetSpeedNormalized());

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(0F, NOISE_AMPLITUDE_MAX, GetSpeedNormalized());
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = Mathf.Lerp(0F, NOISE_FREQUENCY_MAX, GetSpeedNormalized());
    }

    float GetSpeedNormalized()
    {
        return Mathf.Clamp01(_carSpeed.speed / MaxCarSpeed);
    }
}
