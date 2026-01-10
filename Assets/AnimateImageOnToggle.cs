using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimateImageOnToggle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Toggle toggle;

    [Header("Settings")]
    [SerializeField] private float animationDuration = 0.5f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        float targetFill = isOn ? 1f : 0f;
        currentRoutine = StartCoroutine(AnimateFill(targetFill));
    }

    private IEnumerator AnimateFill(float target)
    {
        float start = fillImage.fillAmount;
        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            fillImage.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }

        fillImage.fillAmount = target;
    }
}
