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
    public TextMeshProUGUI nameText;

    [ShowInInspector]
    public GameStatistics Stats;

    [Networked]
    public float TypingInterval { get; set; }

    private float typingTimer = 10f;

    [SerializeField]
    private string AIInput = string.Empty;
    private List<string> correctTargetSentence = new List<string>();
    public float speed { get; set; }
    public float distanceTraveled;

    private Vector3 _startPosition;
    private bool _raceFinished;

    public override void Spawned()
    {
        base.Spawned();

        _startPosition = transform.position;

        // To handle the state authority leaving
        NetworkEvents networkEvents = FindObjectOfType<NetworkEvents>();
        networkEvents.PlayerLeft.AddListener(OnPlayerLeft);

        if (Object.HasStateAuthority)
        {
            SetTypingInterval();
        }

        correctTargetSentence = TypingManager.Instance.targetString.Split(" ").ToList();
        StartCoroutine(updateSpeedText());
        UpdateNameText();
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerLeft)
    {
        // When the owner leaves and we become the master client we should start controller the AI
        if (playerLeft == Object.StateAuthority && NetworkRunnerInstance.Instance.Runner.IsSharedModeMasterClient)
            Object.RequestStateAuthority();
    }

    private void Update()
    {
        AssignOwner();
        if (!Object.HasStateAuthority) { return; }
        if (!TypingManager.Instance.gameStarted) { return; }

        // Check if the typing manager is assigned and the AI is ready to type
        if (typingTimer >= TypingInterval && correctTargetSentence.Count > 0)
        {
            // Simulate the AI typing the word
            string nextWord = correctTargetSentence[0];
            AIInput += nextWord + " ";
            correctTargetSentence.Remove(nextWord);

            UpdateAIWordsRpc(AIInput);

            // Reset the typing timer
            typingTimer = 0f;
            StartCoroutine(lerpSpeed());
        }

        // Check if the AI has finished the race
        distanceTraveled = Vector3.Distance(_startPosition, transform.position);
        if (distanceTraveled >= GameManager.Instance.RaceDistance && !_raceFinished)
        {
            _raceFinished = true;
            AiFinishedRaceRpc(NetworkGameManager.Instance.ElapsedNetworkTime, nameText.text);
        }

        // Update the typing timer
        typingTimer += Time.deltaTime;

        transform.Translate(Vector3.forward * speed * GameManager.Instance.conversionFactor * Time.deltaTime);
    }

    IEnumerator lerpSpeed()
    {
        float elapsedTime = 0;
        float startSpeed = speed;
        Stats = TypingManager.Instance.calculator.CalculateAIGameStatistics(AIInput, TypingManager.Instance.targetString, TypingManager.Instance.getGameTime());
        float targetSpeed = Stats.wordsPerMinute / TypingManager.Instance.fastestWPM * TypingManager.Instance.fastestCarSpeed;

        while (elapsedTime < TypingInterval)
        {
            elapsedTime += Runner.DeltaTime;
            speed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / TypingInterval);
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
                TypingInterval = GameManager.Instance.veryEasyTypingInterval;
                break;
            case DifficultyLevel.Easy:
                TypingInterval = GameManager.Instance.easyTypingInterval;
                break;
            case DifficultyLevel.Medium:
                TypingInterval = GameManager.Instance.mediumTypingInterval;
                break;
            case DifficultyLevel.Hard:
                TypingInterval = GameManager.Instance.hardTypingInterval;
                break;
            case DifficultyLevel.VeryHard:
                TypingInterval = GameManager.Instance.veryHardTypingInterval;
                break;
            case DifficultyLevel.Expert:
                TypingInterval = GameManager.Instance.expertTypingInterval;
                break;
        }

        // Add a random factor to the typing interval
        float randomFactor = Random.Range(GameManager.Instance.minRandomFactor, GameManager.Instance.maxRandomFactor);
        TypingInterval += randomFactor;
    }

    private void UpdateNameText()
    {
        if (nameText == null) { return; }

        switch (difficultyLevel)
        {
            case DifficultyLevel.VeryEasy:
                nameText.text = "AI Very Easy";
                break;
            case DifficultyLevel.Easy:
                nameText.text = "AI Easy";
                break;
            case DifficultyLevel.Medium:
                nameText.text = "AI Medium";
                break;
            case DifficultyLevel.Hard:
                nameText.text = "AI Hard";
                break;
            case DifficultyLevel.VeryHard:
                nameText.text = "AI Very Hard";
                break;
            case DifficultyLevel.Expert:
                nameText.text = "AI Expert";
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void UpdateAIWordsRpc(string words)
    {
        // Don't update if we own this
        if (Object.HasStateAuthority) { return; }

        AIInput = words;
    }

    void AssignOwner()
    {
        if (NetworkRunner.GetRunnerForGameObject(gameObject) == null)
            Debug.Log("Runner is null");

        string debugString = string.Empty;

        if (NetworkRunner.GetRunnerForGameObject(gameObject).IsConnectedToServer)
            debugString += "Connected to server";
        if (NetworkRunner.GetRunnerForGameObject(gameObject).IsSharedModeMasterClient)
            debugString += "\nIs shared mode master";
        if (NetworkRunner.GetRunnerForGameObject(gameObject).IsSceneAuthority)
            debugString += "\nIs Scene authority";
        if (NetworkRunner.GetRunnerForGameObject(gameObject).IsShutdown)
            debugString += "\nIs Shut down";

        //if (Object.StateAuthority. != NetworkRunner.GetRunnerForGameObject(gameObject).)

        Debug.Log(debugString);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void AiFinishedRaceRpc(float time, string name)
    {
        Debug.Log(name + " finished race at " + time);
        RaceLeaderboardManager.Instance.AddEntry(new LeaderboardEntry() { FinishTime = time, Name = name });
    }
}