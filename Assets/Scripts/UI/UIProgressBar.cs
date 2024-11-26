using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    public Image progressBar;
    public float fillDuration = 1f;

    public void SetProgress(float targetValue, System.Action onComplete = null)
    {
        LeanTween.value(gameObject, progressBar.fillAmount, targetValue, fillDuration)
            .setOnUpdate((float value) => progressBar.fillAmount = value)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => onComplete?.Invoke());
    }
}