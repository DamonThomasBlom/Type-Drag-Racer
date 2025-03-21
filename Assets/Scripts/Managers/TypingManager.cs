using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
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
    public GameStatistics LiveGameStats;
    public GameStatistics FinalGameStats;

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

        // On race finished store a snapshot of our live game stats
        GameManager.Instance.OnLocalRaceFinished.AddListener(() => { FinalGameStats = LiveGameStats; });

        targetString = string.Join(" ", generatedWords.ToArray());

        UpdateColors();
        StartCoroutine(updateTextCoroutine());
    }

    #endregion

    void Start()
    {
        ResetTextPosition();
    }

    public RectTransform textTransform; // The RectTransform of the Text component
    public float lineHeight = 30f; // Height of a single line in pixels (adjust based on font size)
    public int wordsPerLine = 10; // Number of words per line
    public float scrollSpeed = 5f; // Speed of the lerp (higher = faster)

    private int currentLine = 0; // Tracks the current line
    private Vector3 targetPosition; // Target position for the text
    private bool isScrolling = false;

    public void OnWordTyped()
    {
        // Call this function every time the user types a word
        int totalWordsTyped = GetTotalWordsTyped(); // Replace with your actual word count logic
        int newLine = (totalWordsTyped - wordsPerLine) / wordsPerLine;
        newLine = newLine < 0 ? 0 : newLine; // Prevent negatives

        // Check if we need to scroll up
        if (newLine != currentLine && newLine < GetTotalLines())
        {
            Debug.Log("Current line - " + newLine);
            currentLine = newLine;
            ScrollText();
        }
    }

    private void ScrollText()
    {
        // Calculate the new target position for the text
        float newYPosition = currentLine * lineHeight;
        targetPosition = new Vector3(textTransform.localPosition.x, newYPosition, textTransform.localPosition.z);

        // Start scrolling
        isScrolling = true;
    }

    private void ResetTextPosition()
    {
        // Reset the text to its starting position
        textTransform.localPosition = Vector3.zero;
        targetPosition = textTransform.localPosition;
        isScrolling = false;
    }

    private int GetTotalWordsTyped()
    {
        int totalWordsTyped = LiveGameStats.totalCharactersTyped / 5;
        return totalWordsTyped; 
    }

    private int GetTotalLines()
    {
        // Calculate the total number of lines based on the total word count
        int totalWords = text.text.Split(' ').Length;
        return Mathf.CeilToInt((float)totalWords / wordsPerLine);
    }

    private void Update()
    {
        // Scroll Text
        if (isScrolling)
        {
            // Smoothly move the text toward the target position
            textTransform.localPosition = Vector3.Lerp(
                textTransform.localPosition,
                targetPosition,
                Time.deltaTime * scrollSpeed
            );

            // Stop scrolling when the text is close enough to the target position
            if (Vector3.Distance(textTransform.localPosition, targetPosition) < 0.1f)
            {
                textTransform.localPosition = targetPosition; // Snap to target to prevent jitter
                isScrolling = false;
            }
        }

        checkInput();

        if (gameStarted)
        {
            gameTimeInSeconds += Time.deltaTime;

            if (gameTimeInSeconds < gameStartTime)
            {
                gameStartTime -= Time.deltaTime * 4;
                LiveGameStats = calculator.CalculateGameStatistics(userInput, targetString, gameStartTime);
            }
            else
                LiveGameStats = calculator.CalculateGameStatistics(userInput, targetString, gameTimeInSeconds);
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
            compareInput();
            yield return null;
        }
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
            {
                if (userInput.Length > 0)
                {
                    userInput = userInput.Substring(0, userInput.Length - 1);
                }
                return;
            }


            if (Input.inputString.Length == 1) 
            {
                userInput += Input.inputString;
            }

            OnWordTyped();
        }
    }

    void compareInput()
    {
        StringBuilder coloredTextBuilder = new StringBuilder();

        int minLength = Mathf.Min(targetString.Length, userInput.Length);

        // Apply monospace
        //coloredTextBuilder.Append("<mspace=0.6em>");

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
        text.SetText(coloredText);
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
