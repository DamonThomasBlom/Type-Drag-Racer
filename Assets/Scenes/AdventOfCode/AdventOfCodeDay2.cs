using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdventOfCodeDay2 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SovlePartOne()
    {
        string[] newInput = input.text.Split('\n');
        Debug.Log("Line count = " + newInput.Length);

        int safeReports = 0;

        foreach (var line in newInput)
        {
            List<int> formatedLine = new List<int>();
            formatedLine.AddRange((line.Split(" ").Select(int.Parse)));

            // Increasing
            if (formatedLine[0] < formatedLine[1])
            {
                bool isSafe = true;

                for (int i = 0; i < formatedLine.Count - 1; i++)
                {
                    if (formatedLine[i] > formatedLine[i + 1])
                        isSafe = false;
                    int difference = Mathf.Abs(formatedLine[i] - formatedLine[i + 1]);
                    if (difference > 3 || difference == 0)
                        isSafe = false;
                }

                if (isSafe)
                {
                    Debug.Log("Safe Report " + string.Join(" ", formatedLine));
                    safeReports++;
                }
            }

            // Decreasing
            if (formatedLine[0] > formatedLine[1])
            {
                bool isSafe = true;

                for (int i = 0; i < formatedLine.Count - 1; i++)
                {
                    if (formatedLine[i] < formatedLine[i + 1])
                        isSafe = false;
                    int difference = Mathf.Abs(formatedLine[i] - formatedLine[i + 1]);
                    if (difference > 3 || difference == 0)
                        isSafe = false;
                }

                if (isSafe)
                {
                    Debug.Log("Safe Report " + string.Join(" ", formatedLine));
                    safeReports++;
                }
            }
        }

        Debug.Log("Safe reports: " + safeReports);
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] newInput = input.text.Split('\n');
        Debug.Log("Line count = " + newInput.Length);

        int safeReports = 0;

        foreach (var line in newInput)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            List<int> levels = line.Split(" ").Select(int.Parse).ToList();

            // Check if the report is already safe
            if (IsSafeReport(levels))
            {
                safeReports++;
                continue;
            }

            // Apply the Problem Dampener (try removing each level once)
            bool dampenedSafe = false;
            for (int i = 0; i < levels.Count; i++)
            {
                // Create a copy of the levels without the current level
                List<int> modifiedLevels = new List<int>(levels);
                modifiedLevels.RemoveAt(i);

                // Check if the modified report is safe
                if (IsSafeReport(modifiedLevels))
                {
                    dampenedSafe = true;
                    break;
                }
            }

            if (dampenedSafe)
            {
                safeReports++;
            }
        }

        Debug.Log("Safe reports (with Dampener): " + safeReports);
    }

    private bool IsSafeReport(List<int> levels)
    {
        if (levels.Count < 2) return false;

        bool isIncreasing = levels[0] < levels[1];
        for (int i = 0; i < levels.Count - 1; i++)
        {
            int diff = levels[i + 1] - levels[i];
            if (diff == 0 || Mathf.Abs(diff) > 3) return false; // Invalid difference
            if ((isIncreasing && diff < 0) || (!isIncreasing && diff > 0)) return false; // Invalid trend
        }

        return true;
    }
}
