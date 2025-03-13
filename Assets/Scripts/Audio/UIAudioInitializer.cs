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

        Destroy(gameObject);
    }
}
