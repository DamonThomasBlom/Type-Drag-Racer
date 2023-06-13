using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class TypingManager : MonoBehaviour
{
    [ShowInInspector]
    public List<string> generatedWords = new List<string>();
    public string initialString;
    public string userInput;
    public TextMeshProUGUI text;

    float gameTimeInSeconds = 0f;
    GameStatisticsCalculator calculator;
    bool gameStarted;

    private void Start()
    {
        calculator = GetComponent<GameStatisticsCalculator>();
        generatedWords = WordsManager.Instance.getRandomWords(10);

        foreach(string word in generatedWords)
        {
            initialString += word + " ";
        }
        initialString = initialString.Trim();
    }

    private void Update()
    {
        checkInput();

        if (gameStarted)
        {
            gameTimeInSeconds += Time.deltaTime;
            //if (string.IsNullOrEmpty(userInput) || string.IsNullOrEmpty(initialString))
            //    return;

            calculator.CalculateGameStatistics(userInput, initialString, gameTimeInSeconds);
        }
    }

    void checkInput()
    {
        if (Input.anyKeyDown)
        {
            gameStarted = true;
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                userInput = userInput.Substring(0, userInput.Length - 1);
                return;
            }


            if (Input.inputString.Length == 1) 
            {
                userInput += Input.inputString;
            }
        }

        compareInput();
    }

    void compareInput()
    {
        string coloredText = "";

        int minLength = Mathf.Min(initialString.Length, userInput.Length);

        // Compare each character in the strings
        for (int i = 0; i < minLength; i++)
        {
            if (initialString[i] == userInput[i])
            {
                // Correct character, color it green
                coloredText += "<color=green>" + userInput[i] + "</color>";
            }
            else
            {
                // Incorrect character, color it red
                coloredText += "<color=red>" + initialString[i] + "</color>";
            }
        }

        // Append the remaining characters from the longer string in gray color
        if (initialString.Length > userInput.Length)
        {
            coloredText += "<color=#808080>" + initialString.Substring(userInput.Length) + "</color>";
        }
        else if (userInput.Length > initialString.Length)
        {
            coloredText += "<color=#808080>" + userInput.Substring(initialString.Length) + "</color>";
        }

        // Update the display text with colored characters
        text.text = coloredText;
    }
}
