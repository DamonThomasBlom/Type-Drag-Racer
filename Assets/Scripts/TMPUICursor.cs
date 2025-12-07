using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPUICursor : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI text;
    public RectTransform cursor;
    public Canvas canvas;

    [Header("Cursor Appearance")]
    public Color cursorColor = Color.white;
    public float blinkSpeed = 3f;     // Higher = faster blinking
    public float lerpSpeed = 15f;     // Position smoothing

    private Vector2 targetPosition;

    void Update()
    {
        // Smooth position
        cursor.anchoredPosition = Vector2.Lerp(
            cursor.anchoredPosition,
            targetPosition,
            Time.deltaTime * lerpSpeed
        );

        // Blinking (fade in/out)
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
        var img = cursor.GetComponentInChildren<Image>();
        img.color = new Color(cursorColor.r, cursorColor.g, cursorColor.b, alpha);
    }

    public void MoveCursorToIndex(int index)
    {
        text.ForceMeshUpdate();
        var textInfo = text.textInfo;

        if (textInfo.characterCount == 0)
        {
            SetTargetPosition(Vector2.zero);
            return;
        }

        // Clamp index
        index = Mathf.Clamp(index, 0, textInfo.characterCount - 1);

        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];

        // Baseline position (X = character origin, Y = baseline)
        Vector3 baselineLocalPos = new Vector3(
            charInfo.origin,
            charInfo.baseLine,
            0
        );

        Vector3 worldPos = text.transform.TransformPoint(baselineLocalPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, worldPos),
            null,
            out Vector2 uiLocalPos
        );

        SetTargetPosition(uiLocalPos);
    }

    private void SetTargetPosition(Vector2 pos)
    {
        targetPosition = pos;
    }
}
