using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AdventOfCodeDay11 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SolvePartOne()
    {
        string[] stones = input.text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("stones count = " + stones.Length);

        List<long> intStones = new List<long>();
        foreach (string stone in stones) { intStones.Add(int.Parse(stone)); }

        int blinks = 25;

        for (int i = 1; i <= blinks; i++)
        {
            List<long> tmpStones = new List<long>();

            for (int j = 0; j < intStones.Count; j++)
            {
                long currentStone = intStones[j];
                string strCurrentStone = currentStone.ToString();

                // First rule
                if (currentStone == 0)
                    tmpStones.Add(1);
                else if (strCurrentStone.Length % 2 == 0) // Second rule
                {
                    // Add first half
                    tmpStones.Add(long.Parse(strCurrentStone.Substring(0, strCurrentStone.Length / 2)));
                    // Add second half
                    tmpStones.Add(long.Parse(strCurrentStone.Substring((strCurrentStone.Length / 2))));
                }
                else // Third rule
                {
                    tmpStones.Add(currentStone * 2024);
                }
            }

            intStones = tmpStones;
            Debug.Log($"After {i} blinks: stones {intStones.Count}");
            //intStones.DebugList();
        }
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] stones = input.text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("stones count = " + stones.Length);

        List<Queue<long>> listOfStones = new List<Queue<long>>();
        foreach (string stone in stones)
        {
            Queue<long> queue = new Queue<long>();
            queue.Enqueue(long.Parse(stone));
            listOfStones.Add(queue);
        }

        int blinks = 75;

        for (int i = 1; i <= blinks; i++)
        {
            long totalStones = 0;
            for (int j = 0; j < listOfStones.Count; j++)
            {
                listOfStones[j] = ProccessQueue(listOfStones[j]);
                totalStones += listOfStones[j].Count;
            }

            Debug.Log($"After {i} blinks: stones {totalStones}");
        }
    }

    Queue<long> ProccessQueue(Queue<long> stoneQueue)
    {
        int initialCount = stoneQueue.Count;

        for (int j = 0; j < initialCount; j++)
        {
            long currentStone = stoneQueue.Dequeue();
            string strCurrentStone = currentStone.ToString();

            // First rule
            if (currentStone == 0)
            {
                stoneQueue.Enqueue(1);
            }
            else if (strCurrentStone.Length % 2 == 0) // Second rule
            {
                // Add first half
                stoneQueue.Enqueue(long.Parse(strCurrentStone.Substring(0, strCurrentStone.Length / 2)));
                // Add second half
                stoneQueue.Enqueue(long.Parse(strCurrentStone.Substring(strCurrentStone.Length / 2)));
            }
            else // Third rule
            {
                stoneQueue.Enqueue(currentStone * 2024);
            }
        }

        return stoneQueue;
    }

    (Queue<T>, Queue<T>) SplitQueue<T>(Queue<T> originalQueue)
    {
        int totalCount = originalQueue.Count;
        int halfCount = totalCount / 2;

        Queue<T> firstHalf = new Queue<T>();
        Queue<T> secondHalf = new Queue<T>();

        for (int i = 0; i < totalCount; i++)
        {
            T item = originalQueue.Dequeue();
            if (i < halfCount)
            {
                firstHalf.Enqueue(item);
            }
            else
            {
                secondHalf.Enqueue(item);
            }
        }

        return (firstHalf, secondHalf);
    }

    [Button]
    public void PartTwo()
    {
        string[] inputStones = input.text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<long, long> stoneCounts = new Dictionary<long, long>();

        // Initialize the dictionary
        foreach (string stone in inputStones)
        {
            long num = long.Parse(stone);
            if (!stoneCounts.ContainsKey(num))
                stoneCounts[num] = 0;
            stoneCounts[num]++;
        }

        int blinks = 75;

        // Simulate the blinks
        for (int i = 1; i <= blinks; i++)
        {
            Dictionary<long, long> newStoneCounts = new Dictionary<long, long>();

            foreach (var pair in stoneCounts)
            {
                long stone = pair.Key;
                long count = pair.Value;

                if (stone == 0)
                {
                    AddStone(newStoneCounts, 1, count);
                }
                else if (stone.ToString().Length % 2 == 0)
                {
                    string strStone = stone.ToString();
                    long left = long.Parse(strStone.Substring(0, strStone.Length / 2));
                    long right = long.Parse(strStone.Substring(strStone.Length / 2));
                    AddStone(newStoneCounts, left, count);
                    AddStone(newStoneCounts, right, count);
                }
                else
                {
                    AddStone(newStoneCounts, stone * 2024, count);
                }
            }

            stoneCounts = newStoneCounts;
            Debug.Log("Dictonary Count: " + stoneCounts.Count);
            Debug.Log($"After {i} blinks: Total stones = {TotalStones(stoneCounts)}");
        }

        string output = string.Empty;
        foreach(var kvp in stoneCounts)
        {
            output += $"KEY: {kvp.Key} VALUE: {kvp.Value}\n";
        }

        Debug.Log(output);
    }

    private void AddStone(Dictionary<long, long> stoneCounts, long stone, long count)
    {
        if (!stoneCounts.ContainsKey(stone))
            stoneCounts[stone] = 0;
        stoneCounts[stone] += count;
    }

    private long TotalStones(Dictionary<long, long> stoneCounts)
    {
        long total = 0;
        foreach (var count in stoneCounts.Values)
        {
            total += count;
        }

        return total;
    }
}