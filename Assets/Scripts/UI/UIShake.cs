using UnityEngine;

public class UIShake : MonoBehaviour
{
    public float intensity = 10f;
    public float duration = 0.5f;

    public void Shake(System.Action onComplete = null)
    {
        LeanTween.moveLocalX(gameObject, transform.localPosition.x + intensity, duration / 2)
                 .setEaseShake()
                 .setLoopPingPong(1)
                 .setOnComplete(() => onComplete?.Invoke());
    }
}
