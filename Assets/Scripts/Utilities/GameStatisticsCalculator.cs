using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public struct GameStatistics
{
    public int totalCharactersTyped;
    public int correctCharactersTyped;
    public int incorrectCharactersTyped;
    public float accuracy;
    public float typingSpeed;
    public int wordsPerMinute;
}

public class GameStatisticsCalculator : MonoBehaviour
{
    public TextMeshProUGUI statisticsText;

    public GameStatistics CalculateAIGameStatistics(string userInput, string initialString, float gameTimeInSeconds)
    {
        GameStatistics statistics = new GameStatistics();

        // Calculate the total number of characters typed
        statistics.totalCharactersTyped = userInput.Length;

        // Calculate the number of correct and incorrect characters typed
        int minLength = Mathf.Min(initialString.Length, userInput.Length);
        for (int i = 0; i < minLength; i++)
        {
            if (initialString[i] == userInput[i])
            {
                statistics.correctCharactersTyped++;
            }
            else
            {
                statistics.incorrectCharactersTyped++;
            }
        }

        // Calculate accuracy
        statistics.accuracy = (float)statistics.correctCharactersTyped / statistics.totalCharactersTyped * 100f;

        // Calculate typing speed in characters per minute
        statistics.typingSpeed = ((float)statistics.totalCharactersTyped - (float)statistics.incorrectCharactersTyped) / gameTimeInSeconds * 60f;

        // Calculate words per minute (assuming average word length of 5 characters)
        statistics.wordsPerMinute = Mathf.RoundToInt(statistics.typingSpeed / 5f);

        return statistics;
    }

    public GameStatistics CalculateGameStatistics(string userInput, string initialString, float gameTimeInSeconds)
    {
        GameStatistics statistics = new GameStatistics();

        // Calculate the total number of characters typed
        statistics.totalCharactersTyped = userInput.Length;

        // Calculate the number of correct and incorrect characters typed
        int minLength = Mathf.Min(initialString.Length, userInput.Length);
        for (int i = 0; i < minLength; i++)
        {
            if (initialString[i] == userInput[i])
            {
                statistics.correctCharactersTyped++;
            }
            else
            {
                statistics.incorrectCharactersTyped++;
            }
        }

        // Calculate accuracy
        statistics.accuracy = (float)statistics.correctCharactersTyped / statistics.totalCharactersTyped * 100f;

        // Calculate typing speed in characters per minute
        statistics.typingSpeed = ((float)statistics.totalCharactersTyped - (float)statistics.incorrectCharactersTyped) / gameTimeInSeconds * 60f;

        // Calculate words per minute (assuming average word length of 5 characters)
        statistics.wordsPerMinute = Mathf.RoundToInt(statistics.typingSpeed / 5f);

        // Update the UI Text component with the statistics
        statisticsText.text = "Total Characters Typed: " + statistics.totalCharactersTyped
            + "\nCorrect Characters Typed: " + statistics.correctCharactersTyped
            + "\nIncorrect Characters Typed: " + statistics.incorrectCharactersTyped
            + "\nAccuracy: " + statistics.accuracy.ToString("F1") + "%"
            + "\nTyping Speed: " + statistics.typingSpeed.ToString("F1") + " characters per minute"
            + "\nWords Per Minute: " + statistics.wordsPerMinute;

        return statistics;
    }
}
