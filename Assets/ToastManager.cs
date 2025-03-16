using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;
    public GameObject toastPrefab;
    public Transform toastParent;
    public float fadeDuration = 0.5f;
    public float defaultDuration = 2f;
    public float verticalSpacing = 50f;

    private List<GameObject> activeToasts = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [Button]
    public void ShowToast(string message, float duration = -1f)
    {
        if (duration < 0) duration = defaultDuration;

        GameObject newToast = Instantiate(toastPrefab, toastParent);
        TextMeshProUGUI toastText = newToast.GetComponentInChildren<TextMeshProUGUI>();
        CanvasGroup canvasGroup = newToast.GetComponent<CanvasGroup>();

        toastText.text = message;
        newToast.transform.localPosition = new Vector3(0, -verticalSpacing * activeToasts.Count, 0);
        activeToasts.Add(newToast);

        canvasGroup.alpha = 0;
        LeanTween.alphaCanvas(canvasGroup, 1, fadeDuration).setOnComplete(() =>
        {
            LeanTween.delayedCall(duration, () =>
            {
                LeanTween.alphaCanvas(canvasGroup, 0, fadeDuration).setOnComplete(() =>
                {
                    activeToasts.Remove(newToast);
                    Destroy(newToast);
                    UpdateToastPositions();
                });
            });
        });
    }

    private void UpdateToastPositions()
    {
        for (int i = 0; i < activeToasts.Count; i++)
        {
            LeanTween.moveLocalY(activeToasts[i], -verticalSpacing * i, 0.3f);
        }
    }
}
