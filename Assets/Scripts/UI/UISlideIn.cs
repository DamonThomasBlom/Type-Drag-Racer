using Sirenix.OdinInspector;
using UnityEngine;

public class UISlideIn : MonoBehaviour
{
    public Vector3 offScreenPosition;
    public Vector3 onScreenPosition;
    public float animationDuration = 0.5f;
    public LeanTweenType easeType = LeanTweenType.easeOutQuad;

    public bool On;

    private void Awake()
    {
        if (On)
            transform.localPosition = onScreenPosition;
        else
            transform.localPosition = offScreenPosition;
    }

    public void SlideIn(System.Action onComplete = null)
    {
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, onScreenPosition, animationDuration).setEase(easeType)
            .setOnComplete(() => {
                On = true;
                onComplete?.Invoke();
                });
    }

    public void SlideOut(System.Action onComplete = null)
    {
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, offScreenPosition, animationDuration).setEase(easeType)
            .setOnComplete(() => {
                On = false;
                onComplete?.Invoke();
                });
    }

    [Button]
    public void SaveCurrentOnPosition()
    {
        onScreenPosition = transform.localPosition;
    }

    [Button]
    public void SaveCurrentOffPosition()
    {
        offScreenPosition = transform.localPosition;
    }
}
