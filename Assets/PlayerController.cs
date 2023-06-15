using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float baseSpeed = 5f; // Base speed of the player/car
    public float speedMultiplier = 2f; // Multiplier for typing speed
    public TextMeshProUGUI speedText;

    private void Update()
    {
        // Calculate the current speed based on the typing speed
        //float currentSpeed = baseSpeed + (typingManager.GameStats.typingSpeed * speedMultiplier);
        //float currentSpeed = typingManager.GameStats.wordsPerMinute;
        float currentSpeed = (TypingManager.Instance.GameStats.wordsPerMinute / TypingManager.Instance.averageCPM) * TypingManager.Instance.averageCarSpeed;

        speedText.text = "km/h: " + currentSpeed.ToString("F0"); 

        // Move the player forward based on the current speed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
