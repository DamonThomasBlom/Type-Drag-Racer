using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    public void FadeIn(System.Action onComplete = null)
    {
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration).setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() => onComplete?.Invoke());
    }

    public void FadeOut(System.Action onComplete = null)
    {
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration).setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => onComplete?.Invoke());
    }
}
