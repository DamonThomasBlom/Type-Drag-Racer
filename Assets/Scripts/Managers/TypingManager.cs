using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GameStatisticsCalculator;
using ColorUtility = UnityEngine.ColorUtility;
using Input = UnityEngine.Input;

public class TypingManager : MonoBehaviour
{
    [ShowInInspector]
    public List<string> generatedWords = new List<string>();
    public string targetString;
    public string userInput;
    public TextMeshProUGUI text;
    public float cursorBlinkInterval = 0.5f;
    public float gameStartTime = 30;

    [ShowInInspector]
    public GameStatistics GameStats;

    public GameStatisticsCalculator calculator;

    public float averageCPM = 500;
    public float averageCarSpeed = 300; // kph

    [HideInInspector]
    public bool gameStarted;

    float gameTimeInSeconds = 0f;

    [SerializeField]
    private Color correctColor = Color.green;

    [SerializeField]
    private Color incorrectColor = Color.red;

    [SerializeField]
    private Color remainingColor = Color.grey;

    [SerializeField]
    private Color cursorColor = Color.yellow;

    private bool cursorVisible = true;
    private float cursorBlinkTimer = 0f;

    string correctColorHex;
    string incorrectColorHex;
    string remainingColorHex;
    string cursorColorHex;

    #region SINGLETON

    public static TypingManager Instance;

    private void Awake()
    {
        Instance = this;
        calculator = GetComponent<GameStatisticsCalculator>();
        generatedWords = WordsManager.Instance.getRandomWords(20);

        targetString = string.Join(" ", generatedWords.ToArray());

        UpdateColors();
        StartCoroutine(updateTextCoroutine());
    }

    #endregion

    private void Update()
    {
        checkInput();

        if (gameStarted)
        {
            gameTimeInSeconds += Time.deltaTime;

            if (gameTimeInSeconds < gameStartTime)
            {
                gameStartTime -= Time.deltaTime * 2;
                GameStats = calculator.CalculateGameStatistics(userInput, targetString, gameStartTime);
            }
            else
                GameStats = calculator.CalculateGameStatistics(userInput, targetString, gameTimeInSeconds);
        }

        // Update the cursor blink
        cursorBlinkTimer += Time.deltaTime;
        if (cursorBlinkTimer >= cursorBlinkInterval)
        {
            cursorVisible = !cursorVisible;
            cursorBlinkTimer = 0f;
        }
    }

    public float getGameTime()
    {
        if (gameTimeInSeconds < gameStartTime)
            return gameStartTime;
        else
            return gameTimeInSeconds;
    }

    IEnumerator updateTextCoroutine()
    {
        while (true)
        {
            compareInput();
            yield return new WaitForSeconds(0.1f);
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
    }

    void compareInput()
    {
        string coloredText = "";

        int minLength = Mathf.Min(targetString.Length, userInput.Length);

        // Compare each character in the strings
        for (int i = 0; i < minLength; i++)
        {
            if (targetString[i] == userInput[i])
            {
                // Correct character, color it green
                coloredText += $"<color={correctColorHex}>{userInput[i]}</color>";
            }
            else
            {
                // Incorrect character, color it red
                coloredText += $"<color={incorrectColorHex}>{targetString[i]}</color>";
            }
        }

        // Add the cursor character if it should be visible
        coloredText += GetCursorCharacter();

        // Append the remaining characters from the longer string in gray color
        if (targetString.Length > userInput.Length)
        {
            coloredText += $"<color={remainingColorHex}>{targetString.Substring(userInput.Length)}</color>";
        }
        else if (userInput.Length > targetString.Length)
        {
            coloredText += $"<color={remainingColorHex}>{userInput.Substring(targetString.Length)}</color>";
        }



        // Update the display text with colored characters
        text.text = coloredText;
    }

    private void UpdateColors()
    {
        correctColorHex = "#" + ColorUtility.ToHtmlStringRGB(correctColor);
        incorrectColorHex = "#" + ColorUtility.ToHtmlStringRGB(incorrectColor);
        remainingColorHex = "#" + ColorUtility.ToHtmlStringRGB(remainingColor);
        cursorColorHex = "#" + ColorUtility.ToHtmlStringRGB(cursorColor);
    }

    private string GetCursorCharacter()
    {
        // Customize the cursor character as needed
        if (cursorVisible)
            return $"<color={cursorColorHex}>|</color>";
        else
            return "|";
    }
}
