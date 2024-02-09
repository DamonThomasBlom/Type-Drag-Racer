using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarAudio : MonoBehaviour
{
    public float minPitch = 0.8f;
    public float maxPitch = 5;
    public float pitchMultiplier = 1; 
    public float maxRandomness = 0.1f;

    private ICarSpeed carSpeed;
    //[SerializeField]
    private float maxCarSpeed;
    private AudioSource audioSource;

    private void Awake()
    {
        carSpeed = GetComponentInParent<ICarSpeed>();
        audioSource = GetComponent<AudioSource>();
        var typeManager = FindObjectOfType<TypingManager>();
        if (typeManager != null) { maxCarSpeed = typeManager.fastestCarSpeed; }

        // Randomize values
        minPitch += Random.Range(-maxRandomness, maxRandomness);
        maxPitch += Random.Range(-maxRandomness, maxRandomness);
    }

    private void Update()
    {
        float pitch = Mathf.Lerp(minPitch, maxPitch, carSpeed.speed / maxCarSpeed);
        audioSource.pitch = pitch * pitchMultiplier;
    }
}
