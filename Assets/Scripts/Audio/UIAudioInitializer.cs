using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioInitializer : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private void Start()
    {
        foreach (Button button in FindObjectsOfType<Button>(true))
        {
            UISoundEffects soundEffects = button.gameObject.AddComponent<UISoundEffects>();
            soundEffects.hoverSound = hoverSound;
            soundEffects.clickSound = clickSound;
        }

        foreach(ButtonManager buttonManager in FindObjectsOfType<ButtonManager>(true))
        {
            UISoundEffects soundEffects = buttonManager.gameObject.AddComponent<UISoundEffects>();
            soundEffects.hoverSound = hoverSound;
            soundEffects.clickSound = clickSound;
        }

        foreach (Slider slider in FindObjectsOfType<Slider>(true))
        {
            UISoundEffects soundEffects = slider.gameObject.AddComponent<UISoundEffects>();
            soundEffects.hoverSound = hoverSound;
            soundEffects.clickSound = clickSound;
        }

        foreach (Toggle togglet in FindObjectsOfType<Toggle>(true))
        {
            UISoundEffects soundEffects = togglet.gameObject.AddComponent<UISoundEffects>();
            soundEffects.hoverSound = hoverSound;
            soundEffects.clickSound = clickSound;
        }

        Destroy(gameObject);
    }
}
