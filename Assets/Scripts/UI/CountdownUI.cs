using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
    public static CountdownUI Instance;

    [Header("References")]
    public TextMeshProUGUI countdownTxt;

    [Header("Settings")]
    public float startScale;
    public float targetScale;
    public float duration;

    public Color startColour;
    public Color targetColour;

    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void CountDownValue(int value)
    {
        LeanTween.cancel(countdownTxt.gameObject);
        countdownTxt.gameObject.SetActive(true);
        switch (value)
        {
            case 3:
            case 2:
            case 1:
                countdownTxt.text = value.ToString();
                countdownTxt.transform.localScale = Vector3.one * startScale;

                LeanTween.scale(countdownTxt.gameObject, Vector3.one * targetScale, duration)
                    .setOnComplete(() => countdownTxt.gameObject.SetActive(false));
                break;
            case 0:
                countdownTxt.text = "GO!";
                countdownTxt.color = startColour;
                countdownTxt.transform.localScale = Vector3.one * startScale;

                // Tween the color of the TextMeshPro component
                LeanTween.value(countdownTxt.gameObject, countdownTxt.color, targetColour, duration * 2)
                    .setOnUpdate((Color value) => { countdownTxt.color = value; })
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() =>
                    {
                        countdownTxt.color = startColour;
                        countdownTxt.gameObject.SetActive(false);
                    });

                // Tween scale
                LeanTween.scale(countdownTxt.gameObject, Vector3.one * targetScale, duration * 2)
                    .setOnComplete(() => countdownTxt.gameObject.SetActive(false));
                break;
        }

    }
}
