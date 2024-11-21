using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static GameStatisticsCalculator;
using ColorUtility = UnityEngine.ColorUtility;
using Input = UnityEngine.Input;

public class TypingManager : MonoBehaviour
{
    //[ShowInInspector]
    private List<string> generatedWords = new List<string>();
    [HideInInspector]
    public string targetString;
    //[HideInInspector]
    [TextArea]
    public string userInput;
    public TextMeshProUGUI text;
    //public TMP_InputField inputField;
    public float cursorBlinkInterval = 0.5f;
    public float gameStartTime = 30;

    //[ShowInInspector]
    public GameStatistics GameStats;

    [HideInInspector]
    public GameStatisticsCalculator calculator;

    public int targetWordCount = 50;
    public float fastestWPM = 500;
    public float fastestCarSpeed = 300; // kph

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
        generatedWords = WordsManager.Instance.getRandomWords(targetWordCount);

        targetString = string.Join(" ", generatedWords.ToArray());

        UpdateColors();
        StartCoroutine(updateTextCoroutine());
        //inputField.caretColor = cursorColor;
        SelectInputField();
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
                gameStartTime -= Time.deltaTime * 4;
                GameStats = calculator.CalculateGameStatistics(userInput, targetString, gameStartTime);
            }
            else
                GameStats = calculator.CalculateGameStatistics(userInput, targetString, gameTimeInSeconds);
        }

        if (cursorBlinkTimer == 0)
        {
            cursorVisible = true;
            return;
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

    int currentWordIndex = 0;

    //[Button]
    //public void nextWord()
    //{
    //    if (currentWordIndex < generatedWords.Count)
    //    {
    //        userInput += generatedWords[currentWordIndex] + " ";
    //        currentWordIndex++;
    //    }
    //}

    [Button]
    public void nextWord()
    {
        int userInputLength = userInput.Length;
        int targetStringLength = targetString.Length;

        if ((userInputLength + 5) <= targetStringLength)
        {
            userInput = targetString.Substring(0, userInputLength + 5);
        }
        else
        {
            userInput = targetString;
        }
    }

    [Button]
    public void previousWord()
    {
        if (currentWordIndex > 0)
        {
            // Find the index of the last space character in the userInput string
            userInput = userInput.Trim();
            int lastSpaceIndex = userInput.LastIndexOf(" ");

            // Remove the last word (including the space) from the userInput string
            userInput = userInput.Substring(0, lastSpaceIndex) + " ";

            currentWordIndex--;
        }
    }

    IEnumerator updateTextCoroutine()
    {
        while (true)
        {
            //if (!inputField.isFocused)
            //{
            //    SelectInputField();
            //}

            compareInput();
            yield return null;
        }
    }

    private void SelectInputField()
    {
        // Set the input field as the currently selected object
        //inputField.Select();
        //inputField.ActivateInputField();
    }

    void checkInput()
    {
        if (!gameStarted) { return; }

        if (Input.anyKeyDown)
        {
            gameStarted = true;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("Enter key pressed!");
                // Handle Enter key logic here
                return;
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            //if (Input.GetKey(KeyCode.Backspace))
            {
                if (userInput.Length > 0)
                {
                    userInput = userInput.Substring(0, userInput.Length - 1);
                    //compareInput();
                }
                return;
            }


            if (Input.inputString.Length == 1) 
            {
                //Debug.Log("User input: " + (string.IsNullOrWhiteSpace(Input.inputString) ? "White space" : Input.inputString));
                userInput += Input.inputString;
                //compareInput();
            }
        }
    }

    void compareInput()
    {
        StringBuilder coloredTextBuilder = new StringBuilder();

        int minLength = Mathf.Min(targetString.Length, userInput.Length);

        //// Compare each character in the strings
        //for (int i = 0; i < minLength; i++)
        //{
        //    if (targetString[i] == userInput[i])
        //    {
        //        // Correct character, color it green
        //        coloredTextBuilder.Append($"<color={correctColorHex}>{userInput[i]}</color>");
        //    }
        //    else
        //    {
        //        // Incorrect character, color it red
        //        if (string.IsNullOrWhiteSpace(targetString[i].ToString()))
        //            coloredTextBuilder.Append($"<color={incorrectColorHex}>{userInput[i]}</color>");
        //        else
        //            coloredTextBuilder.Append($"<color={incorrectColorHex}>{targetString[i]}</color>");
        //    }
        //}

        // Apply monospace
        //coloredTextBuilder.Append("<mspace=0.75em>");

        bool isCorrectSequence = false;
        bool isIncorrectSequence = false;

        // Compare each character in the strings
        for (int i = 0; i < minLength; i++)
        {
            if (targetString[i] == userInput[i])
            {
                if (isCorrectSequence)
                    coloredTextBuilder.Append(userInput[i]);
                else
                {
                    if (isIncorrectSequence)
                        // Close off incorrect colour
                        coloredTextBuilder.Append($"</color>");

                    coloredTextBuilder.Append($"<color={correctColorHex}>{userInput[i]}");
                }
                isCorrectSequence = true;
                isIncorrectSequence = false;
            }
            else
            {
                char appendChar = string.IsNullOrWhiteSpace(targetString[i].ToString()) ? userInput[i] : targetString[i];

                if (isIncorrectSequence)
                    coloredTextBuilder.Append(appendChar);
                else
                {
                    if (isCorrectSequence)
                        // Close off correct colour
                        coloredTextBuilder.Append($"</color>");

                    coloredTextBuilder.Append($"<color={incorrectColorHex}>{appendChar}");
                }

                isCorrectSequence = false;
                isIncorrectSequence = true;
            }
        }
        // Close off any colours
        coloredTextBuilder.Append($"</color>");

        //coloredTextBuilder.Append("</mspace>");
        coloredTextBuilder.Append(GetCursorCharacter());
        //coloredTextBuilder.Append("<mspace=0.75em>");

        // Append the remaining characters from the longer string in gray color
        if (targetString.Length > userInput.Length)
        {
            coloredTextBuilder.Append($"<color={remainingColorHex}>{targetString.Substring(userInput.Length)}</color>");
        }
        else if (userInput.Length > targetString.Length)
        {
            coloredTextBuilder.Append($"<color={remainingColorHex}>{userInput.Substring(targetString.Length)}</color>");
        }

        // Update the display text with colored characters
        string coloredText = coloredTextBuilder.ToString();
        //inputField.text = coloredText;
        //inputField.SetTextWithoutNotify(coloredText);
        text.SetText(coloredText);

        //if (inputField.caretPosition != minLength)
        //    inputField.caretPosition = minLength;
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
