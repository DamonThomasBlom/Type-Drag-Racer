using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public float currentSpeed;

    private void FixedUpdate()
    {
        currentSpeed = TypingManager.Instance.GameStats.wordsPerMinute / TypingManager.Instance.fastestWPM * TypingManager.Instance.fastestCarSpeed;

        speedText.text = "km/h: " + currentSpeed.ToString("F0"); 

        // Move the player forward based on the current speed
        transform.Translate(Vector3.forward * currentSpeed * GameManager.Instance.conversionFactor * Time.deltaTime);
    }
}
