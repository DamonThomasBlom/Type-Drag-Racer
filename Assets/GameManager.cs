using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AIPlayer;

public class GameManager : MonoBehaviour
{
    #region SINGLETON

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            // If instance is null, try to find an existing instance in the scene
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                // If no instance is found, create a new GameObject and attach the SingletonExample component to it
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    instance = singletonObject.AddComponent<GameManager>();
                }

                // Make sure the instance persists across scene changes
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private void Awake()
    {
        // If another instance of SingletonExample already exists, destroy this instance
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [Space(10f)]
    [Header("Difficulty Settings")]
    public float veryEasyTypingInterval = 3f; // Base typing interval for the very easy difficulty
    public float easyTypingInterval = 2f; // Base typing interval for the easy difficulty
    public float mediumTypingInterval = 1.5f; // Base typing interval for the medium difficulty
    public float hardTypingInterval = 1f; // Base typing interval for the hard difficulty
    public float veryHardTypingInterval = 0.75f; // Base typing interval for the very hard difficulty
    public float expertTypingInterval = 0.5f; // Base typing interval for the expert difficulty
    public float minRandomFactor = -0.2f; // Minimum random factor for the typing interval
    public float maxRandomFactor = 0.2f; // Maximum random factor for the typing interval

    // This converts a KM/H to units inside of unity
    public float conversionFactor = 0.27778f;

    [Button]
    public void CalculateAverageWordsPerMinute()
    {
        AIPlayer[] aiBots = FindObjectsOfType<AIPlayer>();
        Dictionary<DifficultyLevel, float> totalWPMs = new Dictionary<DifficultyLevel, float>();
        Dictionary<DifficultyLevel, int> botCounts = new Dictionary<DifficultyLevel, int>();

        foreach (AIPlayer bot in aiBots)
        {
            float botWPM = bot.Stats.wordsPerMinute;
            DifficultyLevel difficulty = bot.difficultyLevel;

            if (totalWPMs.ContainsKey(difficulty))
            {
                totalWPMs[difficulty] += botWPM;
                botCounts[difficulty]++;
            }
            else
            {
                totalWPMs[difficulty] = botWPM;
                botCounts[difficulty] = 1;
            }
        }

        // Sort the totalWPMs dictionary based on the difficulty level
        var sortedWPMs = totalWPMs.OrderBy(kvp => kvp.Key);

        foreach (KeyValuePair<DifficultyLevel, float> kvp in sortedWPMs)
        {
            DifficultyLevel difficulty = kvp.Key;
            float totalWPM = kvp.Value;
            int count = botCounts[difficulty];
            float averageWPM = totalWPM / count;

            Debug.Log(difficulty.ToString() + " Average WPM: " + averageWPM);
        }
    }
}
