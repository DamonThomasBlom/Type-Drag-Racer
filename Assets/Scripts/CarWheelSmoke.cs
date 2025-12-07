using System.Collections;
using UnityEngine;

public class CarWheelSmoke : MonoBehaviour
{
    public ParticleSystem LeftWheelSmokeParticle;
    public ParticleSystem RightWheelSmokeParticle;

    public float SmokeDuration = 5;

    private ICarSpeed carSpeed;

    private void Start()
    {
        StopParticle();

        carSpeed = GetComponentInParent<ICarSpeed>();

        if (NetworkGameManager.Instance == null)
        {
            Debug.LogError("Network game manager is null");
            return;
        }

        NetworkGameManager.Instance.OnGameStarted.AddListener(OnGameStarted);
    }

    private void OnGameStarted()
    {
        StartCoroutine(StartAndStopSmoke());
    }

    IEnumerator StartAndStopSmoke()
    {
        float elapsedTime = 0;

        while (elapsedTime < SmokeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (carSpeed.speed > 0) 
                PlayParticle();
            else
                StopParticle();

            yield return null;
        }

        StopParticle();
    }

    void PlayParticle()
    {
        if (LeftWheelSmokeParticle.isPlaying) return;

        LeftWheelSmokeParticle.Play();
        RightWheelSmokeParticle.Play();
    }

    void StopParticle()
    {
        if (!LeftWheelSmokeParticle.isPlaying) return;

        LeftWheelSmokeParticle.Stop();
        RightWheelSmokeParticle.Stop();
    }
}
