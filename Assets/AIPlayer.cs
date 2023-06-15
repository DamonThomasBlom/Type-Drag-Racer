using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static GameStatisticsCalculator;

public class AIPlayer : MonoBehaviour
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    public DifficultyLevel difficultyLevel; // Enum to represent the difficulty level
    public float easyTypingInterval = 2f; // Typing interval for the easy difficulty
    public float mediumTypingInterval = 1.5f; // Typing interval for the medium difficulty
    public float hardTypingInterval = 1f; // Typing interval for the hard difficulty
    public float minRandomFactor = -0.5f; // Minimum random factor for the typing interval
    public float maxRandomFactor = 0.5f; // Maximum random factor for the typing interval

    public TextMeshProUGUI speedText;

    [ShowInInspector]
    public GameStatistics Stats;

    private float typingTimer = 0f;
    private float typingInterval;
    private string AIInput = string.Empty;
    private List<string> correctTargetSentence = new List<string>();

    private void Start()
    {
        // Set the initial typing interval based on the selected difficulty level
        SetTypingInterval();

        correctTargetSentence = TypingManager.Instance.targetString.Split(" ").ToList();
    }

    private void Update()
    {
        if (!TypingManager.Instance.gameStarted) { return; }

        // Check if the typing manager is assigned and the AI is ready to type
        if (typingTimer >= typingInterval && correctTargetSentence.Count > 0)
        {
            // Simulate the AI typing the word
            string nextWord = correctTargetSentence[0];
            AIInput += nextWord + " ";
            correctTargetSentence.Remove(nextWord);

            // Reset the typing timer
            typingTimer = 0f;
        }

        // Update the typing timer
        typingTimer += Time.deltaTime;

        Stats = TypingManager.Instance.calculator.CalculateGameStatistics(AIInput, TypingManager.Instance.targetString, TypingManager.Instance.getGameTime());

        float currentSpeed = Stats.wordsPerMinute / TypingManager.Instance.averageCPM * TypingManager.Instance.averageCarSpeed;

        speedText.text = "km/h: " + currentSpeed.ToString("F0");

        // Move the player forward based on the current speed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void SetTypingInterval()
    {
        // Set the typing interval based on the selected difficulty level
        switch (difficultyLevel)
        {
            case DifficultyLevel.Easy:
                typingInterval = easyTypingInterval;
                break;
            case DifficultyLevel.Medium:
                typingInterval = mediumTypingInterval;
                break;
            case DifficultyLevel.Hard:
                typingInterval = hardTypingInterval;
                break;
            default:
                typingInterval = easyTypingInterval;
                break;
        }

        // Add a random factor to the typing interval
        float randomFactor = Random.Range(minRandomFactor, maxRandomFactor);
        typingInterval += randomFactor;
    }
}


