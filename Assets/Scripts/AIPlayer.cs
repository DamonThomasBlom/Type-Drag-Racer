using Fusion;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AIPlayer : NetworkBehaviour, ICarSpeed
{
    public enum DifficultyLevel
    {
        VeryEasy,
        Easy,
        Medium,
        Hard,
        VeryHard,
        Expert
    }

    public DifficultyLevel difficultyLevel; // Enum to represent the difficulty level
    public TextMeshProUGUI speedText;

    [ShowInInspector]
    public GameStatistics Stats;

    private float typingTimer = 10f;
    private float typingInterval;
    private string AIInput = string.Empty;
    private List<string> correctTargetSentence = new List<string>();
    public float speed { get; set; }

    //private void Start()
    //{
    //    // Set the initial typing interval based on the selected difficulty level
    //    SetTypingInterval();

    //    correctTargetSentence = TypingManager.Instance.targetString.Split(" ").ToList();
    //    StartCoroutine(updateSpeedText());
    //}

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            SetTypingInterval();

            correctTargetSentence = TypingManager.Instance.targetString.Split(" ").ToList();
        }

        StartCoroutine(updateSpeedText());
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
            StartCoroutine(lerpSpeed());
        }

        // Update the typing timer
        typingTimer += Time.deltaTime;

        transform.Translate(Vector3.forward * speed * GameManager.Instance.conversionFactor * Time.deltaTime);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!TypingManager.Instance.gameStarted) { return; }
        if (!HasStateAuthority) { return; } 


        // Check if the typing manager is assigned and the AI is ready to type
        if (typingTimer >= typingInterval && correctTargetSentence.Count > 0)
        {
            // Simulate the AI typing the word
            string nextWord = correctTargetSentence[0];
            AIInput += nextWord + " ";
            correctTargetSentence.Remove(nextWord);

            // Reset the typing timer
            typingTimer = 0f;
            StartCoroutine(lerpSpeed());
        }

        // Update the typing timer
        typingTimer += Runner.DeltaTime;

        transform.Translate(Vector3.forward * speed * GameManager.Instance.conversionFactor * Runner.DeltaTime);
    }

    //private void FixedUpdate()
    //{
    //    // Move the player forward based on the current speed
    //    transform.Translate(Vector3.forward * currentSpeed * GameManager.Instance.conversionFactor * Time.fixedDeltaTime);
    //}

    IEnumerator lerpSpeed()
    {
        float elapsedTime = 0;
        float startSpeed = speed;
        Stats = TypingManager.Instance.calculator.CalculateAIGameStatistics(AIInput, TypingManager.Instance.targetString, TypingManager.Instance.getGameTime());
        float targetSpeed = Stats.wordsPerMinute / TypingManager.Instance.fastestWPM * TypingManager.Instance.fastestCarSpeed;

        while (elapsedTime < typingInterval)
        {
            elapsedTime += Runner.DeltaTime;
            speed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / typingInterval);
            yield return null;
        }
    }

    IEnumerator updateSpeedText()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            speedText.text = "km/h: " + speed.ToString("F0");
        }
    }

    private void SetTypingInterval()
    {
        // Set the typing interval based on the selected difficulty level
        switch (difficultyLevel)
        {
            case DifficultyLevel.VeryEasy:
                typingInterval = GameManager.Instance.veryEasyTypingInterval;
                break;
            case DifficultyLevel.Easy:
                typingInterval = GameManager.Instance.easyTypingInterval;
                break;
            case DifficultyLevel.Medium:
                typingInterval = GameManager.Instance.mediumTypingInterval;
                break;
            case DifficultyLevel.Hard:
                typingInterval = GameManager.Instance.hardTypingInterval;
                break;
            case DifficultyLevel.VeryHard:
                typingInterval = GameManager.Instance.veryHardTypingInterval;
                break;
            case DifficultyLevel.Expert:
                typingInterval = GameManager.Instance.expertTypingInterval;
                break;
        }

        // Add a random factor to the typing interval
        float randomFactor = Random.Range(GameManager.Instance.minRandomFactor, GameManager.Instance.maxRandomFactor);
        typingInterval += randomFactor;
    }
}