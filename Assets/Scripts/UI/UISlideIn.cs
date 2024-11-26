using UnityEngine;

public class UISlideIn : MonoBehaviour
{
    public Vector3 offScreenPosition;
    public float animationDuration = 0.5f;
    public LeanTweenType easeType = LeanTweenType.easeOutQuad;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        transform.localPosition = offScreenPosition;
    }

    public void SlideIn(System.Action onComplete = null)
    {
        LeanTween.moveLocal(gameObject, originalPosition, animationDuration).setEase(easeType)
            .setOnComplete(() => onComplete?.Invoke());
    }

    public void SlideOut(System.Action onComplete = null)
    {
        LeanTween.moveLocal(gameObject, offScreenPosition, animationDuration).setEase(easeType)
            .setOnComplete(() => onComplete?.Invoke());
    }
}
