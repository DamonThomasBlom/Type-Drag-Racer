using Sirenix.OdinInspector;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    public Vector3 rotationAmount = new Vector3(0, 360, 0); // Rotation per cycle
    public float duration = 2f; // Duration of one cycle
    public bool loop = false; // Toggle looping
    public bool playOnStart;

    private int tweenId;

    private void Start()
    {
        if (playOnStart)
            StartRotation();
    }

    [Button]
    public void StartRotation()
    {
        StopRotation(); // Ensure no duplicate animations

        if (loop)
        {
            tweenId = LeanTween.rotateAround(gameObject, Vector3.up, rotationAmount.y, duration)
                .setLoopClamp() // Keeps looping
                .id;
        }
        else
        {
            tweenId = LeanTween.rotateAround(gameObject, Vector3.up, rotationAmount.y, duration).id;
        }
    }

    [Button]
    public void StopRotation()
    {
        LeanTween.cancel(gameObject); // Cancels any active tween on this object
    }
}
