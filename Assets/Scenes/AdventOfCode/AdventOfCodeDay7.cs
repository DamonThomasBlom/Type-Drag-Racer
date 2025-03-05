using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AdventOfCodeDay7 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SolvePartOne()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Line count = " + lines.Length);

        long totalForValidEquations = 0;

        foreach(var line in lines)
        {
            long total = long.Parse(line.Split(":")[0]);
            List<int> numbers = line.Split(":")[1].Split(' ', StringSplitOptions.RemoveEmptyEntries) // Split by spaces
                         .Select(int.Parse) // Convert each split string to an int
                         .ToList();

            if (CheckIfEquationIsValid(total, numbers))
            {
                Debug.Log("This is a valid equation");
                Debug.Log(string.Join(", ", numbers));
                totalForValidEquations += total;
            }
            // 1  3  5  6
        }

        Debug.Log("Answer: " +  totalForValidEquations);
    }

    bool CheckIfEquationIsValid(long targetValue, List<int> numbers)
    {
        int totalConfigurations = (int)math.pow(2, numbers.Count - 1);

        // We will store every configuration as a list on ints and treat 0 as addition and 1 as multiplication
        List<List<int>> operatorsConfig = new List<List<int>>();

        while (operatorsConfig.Count < totalConfigurations)
        {
            List<int> config = new List<int>();
            for (int i = 0; i < numbers.Count - 1; i++) 
            {
                config.Add(Random.Range(0, 2));
            }

            // Check to make sure we not adding duplicates
            bool containsConfig = false;
            foreach (var _config in operatorsConfig)
            {
                if (AreListsTheSame(config, _config))
                    containsConfig = true;
            }
            if (!containsConfig)
                operatorsConfig.Add(config);
        }

        // Loop through every configuration and see if we can match our total
        foreach(var config in operatorsConfig)
        {
            // Start off on the first number
            long total = numbers[0];
            for(int i = 1;i < numbers.Count;i++) 
            {
                // Addition
                if (config[i - 1] == 0)
                    total += numbers[i];

                // Multiplication
                if (config[i - 1] == 1)
                    total *= numbers[i];
            }

            if (total == targetValue)
                return true;
        }

        return false;
    }

    bool AreListsTheSame(List<int> x, List<int> y)
    {
        bool listAreTheSame = true;

        for(int i = 0;i < x.Count;i++)
        {
            if (x[i] != y[i])
                listAreTheSame = false;
        }

        return listAreTheSame;
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        //string[] lines = input.text.Split("\n");
        Debug.Log("Line count = " + lines.Length);

        long totalForValidEquations = 0;

        _operatorsConfig = new List<List<int>>();

        // Initialize all configs
        for (int i = 2;i <= 12; i++)
        {
            AddConfig(i);
        }

        Debug.Log("Configurations Initialized");

        foreach (var line in lines)
        {
            long total = long.Parse(line.Split(":")[0]);
            List<int> numbers = line.Split(":")[1].Split(' ', StringSplitOptions.RemoveEmptyEntries) // Split by spaces
                         .Select(int.Parse) // Convert each split string to an int
                         .ToList();

            if (CheckIfEquationIsValidPartTwo(total, numbers))
            {
                //Debug.Log("This is a valid equation");
                //Debug.Log(string.Join(", ", numbers));
                totalForValidEquations += total;
            }
            // 1  3  5  6
        }
        Debug.Log("Answer: " + totalForValidEquations);
    }

    private List<List<int>> _operatorsConfig = new List<List<int>>();

    [Button]
    void AddConfig(int numbers)
    {
        var tempConfig = new List<List<int>>();
        int totalConfigurations = (int)math.pow(3, numbers - 1);

        while (tempConfig.Count < totalConfigurations)
        {
            List<int> config = new List<int>();

            if (_operatorsConfig.Count > 0)
            {
                foreach (var previousConfig in _operatorsConfig)
                {
                    if (previousConfig.Count == numbers - 2)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            config = new List<int>(previousConfig) { i };
                            bool _containsConfig = false;
                            foreach (var _config in tempConfig)
                            {
                                if (AreListsTheSame(config, _config))
                                    _containsConfig = true;
                            }
                            if (!_containsConfig)
                                tempConfig.Add(config);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < numbers - 1; i++)
                {
                    config.Add(Random.Range(0, 3));
                }

                // Check to make sure we not adding duplicates
                bool containsConfig = false;
                foreach (var _config in tempConfig)
                {
                    if (AreListsTheSame(config, _config))
                        containsConfig = true;
                }
                if (!containsConfig)
                    tempConfig.Add(config);
            }

            // 0   1   2
            // 0,0   0,1   0,2   1,0   1,1   1,2  2,0  2,1   2,2
        }

        _operatorsConfig.AddRange(tempConfig);
    }

    bool CheckIfEquationIsValidPartTwo(long targetValue, List<int> numbers)
    {
        // We will store every configuration as a list on ints and treat 0 as addition and 1 as multiplication and 2 as concatenation
        List<List<int>> operatorsConfig = new List<List<int>>();

        // Sort through all our configs and only use the relevant ones
        foreach(var config in _operatorsConfig)
        {
            if (config.Count == (numbers.Count - 1))
                operatorsConfig.Add(config);
        }

        //while (operatorsConfig.Count < totalConfigurations)
        //{
        //    List<int> config = new List<int>();
        //    for (int i = 0; i < numbers.Count - 1; i++)
        //    {
        //        config.Add(Random.Range(0, 3));
        //    }

        //    // Check to make sure we not adding duplicates
        //    bool containsConfig = false;
        //    foreach (var _config in operatorsConfig)
        //    {
        //        if (AreListsTheSame(config, _config))
        //            containsConfig = true;
        //    }
        //    if (!containsConfig)
        //        operatorsConfig.Add(config);
        //}

        // Loop through every configuration and see if we can match our total
        foreach (var config in operatorsConfig)
        {
            // Start off on the first number
            long total = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
            {
                // Addition
                if (config[i - 1] == 0)
                    total += numbers[i];

                // Multiplication
                if (config[i - 1] == 1)
                    total *= numbers[i];

                // Concatenate
                if (config[i - 1] == 2)
                    total = long.Parse((total.ToString() + numbers[i].ToString()));
            }

            if (total == targetValue)
                return true;
        }

        return false;
    }
}