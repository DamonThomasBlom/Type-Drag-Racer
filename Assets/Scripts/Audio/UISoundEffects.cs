using UnityEngine;
using UnityEngine.EventSystems;

public class UISoundEffects : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance?.PlayOneShotSound(hoverSound, AudioManager.Instance.Sfx);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance?.PlayOneShotSound(clickSound, AudioManager.Instance.Sfx);
    }
}
