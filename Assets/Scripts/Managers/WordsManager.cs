using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class WordsManager : MonoBehaviour
{
    #region SINGLETON

    private static WordsManager instance;

    public static WordsManager Instance
    {
        get
        {
            // If instance is null, try to find an existing instance in the scene
            if (instance == null)
            {
                instance = FindObjectOfType<WordsManager>();

                // If no instance is found, create a new GameObject and attach the SingletonExample component to it
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("WordsManager");
                    instance = singletonObject.AddComponent<WordsManager>();
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

        loadWords();
    }

    #endregion

    [ShowInInspector]
    public List<string> words = new List<string>();

    private static Random random = new Random();

    public List<string> getRandomWords(int numberOfWords)
    {
        if (numberOfWords <= 0 || numberOfWords > words.Count)
        {
            throw new ArgumentException("Invalid count specified.");
        }

        List<string> randomWords = new List<string>();

        // Shuffle the original word list
        List<string> shuffledList = new List<string>(words);
        ShuffleList(shuffledList);

        // Select the specified number of random words from the shuffled list
        for (int i = 0; i < numberOfWords; i++)
        {
            randomWords.Add(shuffledList[i]);
        }

        return randomWords;
    }

    void loadWords()
    {
        string filePath = "words_200";

        // Load the JSON file as a TextAsset using the Resources class
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile != null)
        {
            // Read the text contents of the JSON file
            string jsonText = jsonFile.text;

            //Debug.Log(jsonText);

            try
            {
                words.AddRange(JsonConvert.DeserializeObject<WordsJson>(jsonText).Words);
            }
            catch(JsonException e)
            {
                Debug.LogError("Json parse exception");
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + filePath);
        }
    }

    private static void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
