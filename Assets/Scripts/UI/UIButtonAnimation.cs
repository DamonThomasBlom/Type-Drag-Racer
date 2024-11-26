using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float hoverScale = 1.1f;
    public float clickScale = 0.9f;
    public float animationDuration = 0.2f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale * hoverScale, animationDuration).setEaseOutQuad()
            .setOnComplete(() => Debug.Log("Hover animation complete"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale, animationDuration).setEaseOutQuad()
            .setOnComplete(() => Debug.Log("Exit hover animation complete"));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale * clickScale, animationDuration / 2).setEaseOutQuad()
            .setOnComplete(() => Debug.Log("Click animation complete"));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale * hoverScale, animationDuration / 2).setEaseOutQuad()
            .setOnComplete(() => Debug.Log("Release animation complete"));
    }
}
