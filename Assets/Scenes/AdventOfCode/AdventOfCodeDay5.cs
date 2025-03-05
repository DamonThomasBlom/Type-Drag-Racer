using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AdventOfCodeDay5 : MonoBehaviour
{
    public TextAsset input;

    public List<int> TestList = new List<int>();

    [Button]
    public void TestMove(int first, int second)
    {
        TestList.Move(first, second);
    }

    [Button]
    public void TestSwap(int first, int second)
    {
        TestList.Move(first, second);
    }

    [Button]
    public void SolvePartOne()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        //string[] lines = input.text.Split("\n");
        Debug.Log("Line count = " + lines.Length);

        TestList = new List<int>();
        int sumOfMiddleNumbers = 0;

        List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();

        foreach(var line in lines)
        {
            if (line.Contains("|"))
            {
                var splitLine = line.Split("|");
                int firstNumber = int.Parse(splitLine[0]);
                int secondNumber = int.Parse(splitLine[1]);
                pairs.Add(new KeyValuePair<int, int>(firstNumber, secondNumber));
            }
        }

        var groups = new Dictionary<int, List<int>>();
        foreach (var (key, value) in pairs)
        {
            if (!groups.ContainsKey(key))
            {
                groups[key] = new List<int>();
            }
            groups[key].Add(value);
        }

        foreach(var line in lines)
        {
            if (line.Contains(","))
            {
                List<int> numbers = line?.Split(',')?.Select(Int32.Parse)?.ToList();
                if (IsValidSequence(numbers, groups))
                    sumOfMiddleNumbers += numbers[numbers.Count / 2];
            }
        }

        Debug.Log("Sum of middle numbers - " + sumOfMiddleNumbers);
    }

    static bool IsValidSequence(List<int> sequence, Dictionary<int, List<int>> groups)
    {
        for (int i = 0; i < sequence.Count - 1; i++)
        {
            int current = sequence[i];
            int next = sequence[i + 1];

            // Check if the dictionary contains the current key
            if (!groups.ContainsKey(current) || !groups[current].Contains(next))
            {
                return false;
            }
        }
        return true;
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        //string[] lines = input.text.Split("\n");
        Debug.Log("Line count = " + lines.Length);

        TestList = new List<int>();
        int sumOfMiddleNumbers = 0;

        List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();

        foreach (var line in lines)
        {
            if (line.Contains("|"))
            {
                var splitLine = line.Split("|");
                int firstNumber = int.Parse(splitLine[0]);
                int secondNumber = int.Parse(splitLine[1]);
                pairs.Add(new KeyValuePair<int, int>(firstNumber, secondNumber));
            }
        }

        var groups = new Dictionary<int, List<int>>();
        foreach (var (key, value) in pairs)
        {
            if (!groups.ContainsKey(key))
            {
                groups[key] = new List<int>();
            }
            groups[key].Add(value);
        }

        foreach (var line in lines)
        {
            if (line.Contains(","))
            {
                List<int> numbers = line?.Split(',')?.Select(Int32.Parse)?.ToList();
                if (!IsValidSequence(numbers, groups))
                {
                    var sortedList = SortOrderOfList(numbers, groups);
                    Debug.Log("Sorted List");
                    sortedList.DebugList();
                    int middleIndex = sortedList.Count / 2;
                    sumOfMiddleNumbers += sortedList[middleIndex];
                }
            }
        }

        Debug.Log("Sum of middle numbers - " + sumOfMiddleNumbers);
    }

    List<int> SortOrderOfList(List<int> currentList, Dictionary<int, List<int>> groups)
    {
        // Continue sorting until the list is fully sorted
        for (int i = 0; i < currentList.Count - 1; i++)
        {
            int current = currentList[i];
            int next = currentList[i + 1];

            // Check if the current number can lead to the next
            if (!groups.ContainsKey(current) || !groups[current].Contains(next))
            {
                // Move the incorrectly positioned number to the end
                int temp = currentList[i];
                currentList.RemoveAt(i);
                currentList.Add(temp);

                // Restart sorting from the beginning
                i = -1;
            }
        }

        return currentList;
    }
}


