using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    public float animationDuration = 0.3f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void ShowPopUp(System.Action onComplete = null)
    {
        LeanTween.scale(gameObject, Vector3.one, animationDuration).setEaseOutBack()
            .setOnComplete(() => onComplete?.Invoke());
    }

    public void HidePopUp(System.Action onComplete = null)
    {
        LeanTween.scale(gameObject, Vector3.zero, animationDuration).setEaseInBack()
            .setOnComplete(() => onComplete?.Invoke());
    }
}
