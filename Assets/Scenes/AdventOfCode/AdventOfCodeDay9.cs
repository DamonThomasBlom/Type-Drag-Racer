using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class AdventOfCodeDay9 : MonoBehaviour
{
    public TextAsset input;

    private Stopwatch sw;

    void StartStopWatch()
    {
        sw = Stopwatch.StartNew();
    }

    void ShowStopWatch()
    {
        if (sw != null)
        {
            Debug.Log(sw.ElapsedMilliseconds + " ms");
            sw.Stop();
            sw = null;
        }
    }

    [Button]
    public void SolvePartOne()
    {
        StartStopWatch();

        List<string> diskMap = input.text.Select(c => c.ToString()).ToList();
        Debug.Log("Line count = " + diskMap.Count);

        ShowStopWatch();
        //diskMap.DebugList();

        StartStopWatch();

        List<string> formatedDiskMap = new List<string>();

        int fileID = 0;

        for (int i = 0; i < diskMap.Count; i++)
        {
            // Every 2nd index is the length of a file
            bool isLengthOfFreeSpace = (i + 1) % 2 == 0;

            if (!isLengthOfFreeSpace)
            {
                int lengthOfFile = int.Parse(diskMap[i]);

                for (int j = 0; j < lengthOfFile; j++)
                {
                    formatedDiskMap.Add(fileID.ToString());
                }
                fileID++;
            }
            else
            {
                int lenghtOfFreeSpace = int.Parse(diskMap[i]);
                for (int j = 0; j < lenghtOfFreeSpace; j++)
                {
                    formatedDiskMap.Add(".");
                }
            }
        }

        Debug.Log("Formated disk");

        ShowStopWatch();
        //formatedDiskMap.DebugList();


        StartStopWatch();
        // Move file blocks right to left
        for (int i = formatedDiskMap.Count - 1; i > 0; i--)
        {
            // Its not an empty space so move it to the front to replace the first . we find
            if (formatedDiskMap[i] != ".")
            {
                // Find the first . in the list
                int swapTo = formatedDiskMap.IndexOf(".");

                // Only swap if its before us
                if (swapTo < i)
                    formatedDiskMap.Swap(swapTo, i);
            }
        }

        Debug.Log("Final clean up");
        ShowStopWatch();
        //formatedDiskMap.DebugList();

        long sumOfCheckSum = 0;

        StartStopWatch();
        // Loop until the first .
        for (int i = 0; i < formatedDiskMap.IndexOf("."); i++)
        {
            sumOfCheckSum += i * int.Parse(formatedDiskMap[i]);
        }

        Debug.Log("Sum of Checksum: " + sumOfCheckSum);
        ShowStopWatch();
    }

    [Button]
    public void SolvePartTwo()
    {
        List<string> diskMap = input.text.Select(c => c.ToString()).ToList();
        Debug.Log("Line count = " + diskMap.Count);

        //diskMap.DebugList();

        List<string> formatedDiskMap = new List<string>();

        int fileID = 0;

        Dictionary<string, List<int>> fileBlocks = new Dictionary<string, List<int>>();

        for (int i = 0; i < diskMap.Count; i++)
        {
            //Debug.Log("Loop");

            // Every 2nd index is the length of a file
            bool isLengthOfFreeSpace = (i + 1) % 2 == 0;

            if (!isLengthOfFreeSpace)
            {
                int lengthOfFile = int.Parse(diskMap[i]);
                string fileBlock = fileID.ToString();

                for (int j = 0; j < lengthOfFile; j++)
                {
                    formatedDiskMap.Add(fileBlock);

                    if (fileBlocks.ContainsKey(fileBlock))
                        fileBlocks[fileBlock].Add(formatedDiskMap.Count - 1);
                    else
                        fileBlocks.Add(fileBlock, new List<int>() { formatedDiskMap.Count - 1 });
                }
                fileID++;
            }
            else
            {
                int lenghtOfFreeSpace = int.Parse(diskMap[i]);
                for (int j = 0; j < lenghtOfFreeSpace; j++)
                {
                    formatedDiskMap.Add(".");
                }
            }
        }

        //Debug.Log("Blocks Dict");
        //string output = string.Empty;
        //foreach(var item in fileBlocks)
        //{
        //    output += $"KEY: {item.Key} VALUE: {string.Join(",", item.Value)}\n";
        //}
        //Debug.Log(output);

        Debug.Log("Formated disk");
        //formatedDiskMap.DebugList();

        // KEY: First index of free space VALUE: consecutive free spaces
        Dictionary<int, List<int>> DotSizes = new Dictionary<int, List<int>>();

        // Move file blocks right to left
        for (int i = formatedDiskMap.Count - 1; i > 0; i--)
        {
            // Its not an empty space so move it to the front to replace the first . we find
            if (formatedDiskMap[i] != ".")
            {
                // Find the first . in the list
                if (!fileBlocks.ContainsKey(formatedDiskMap[i]))
                    continue;

                List<int> currentBlockIndexes = fileBlocks[formatedDiskMap[i]];
                List<int> emptySpace = FindEmptySpacesOfLength(formatedDiskMap, currentBlockIndexes.Count);

                //Debug.Log($"Current block indexes {formatedDiskMap[i]}");
                //currentBlockIndexes.DebugList();

                //Debug.Log("Empty spaces response");
                //emptySpace.DebugList();

                fileBlocks.Remove(formatedDiskMap[i]);

                // We couldn't find a space big enough for the current block
                if (emptySpace.Count == 0 || emptySpace[0] > currentBlockIndexes[0])
                    continue;

                for (int j = 0; j < currentBlockIndexes.Count; j++)
                {
                    formatedDiskMap.Swap(currentBlockIndexes[j], emptySpace[j]);
                }    

                //// This is where its going
                //int swapTo = emptySpace[0];

                //// Completely take out all the dots
                //foreach (var dot in emptySpace)
                //{
                //    i--;
                //    formatedDiskMap.RemoveAt(dot);
                //}

                //Debug.Log($"I {i} : swapto {swapTo} : length {formatedDiskMap.Count}");

                //// Move the block to where the first spot was
                //formatedDiskMap.Move(i, swapTo);
            }
        }

        Debug.Log("Final clean up");
        //formatedDiskMap.DebugList();

        long sumOfCheckSum = 0;

        // Loop until the first .
        for (int i = 0; i < formatedDiskMap.Count; i++)
        {
            if (formatedDiskMap[i] == ".")
                continue;
            sumOfCheckSum += i * int.Parse(formatedDiskMap[i]);
        }

        Debug.Log("Sum of Checksum: " + sumOfCheckSum);
    }

    public List<int> FindEmptySpacesOfLength(List<string> list, int length)
    {
        List<int> result = new List<int>();
        List<int> dotIndexes = new List<int>();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == ".")
            {
                dotIndexes.Add(i);
            }
        }

        // Make sure we still have dots
        if (dotIndexes.Count == 0)
            return result;

        if (length == 1)
        {
            result.Add(dotIndexes[0]);
            return result;
        }

        for (int i = 0; i < dotIndexes.Count - 1; i++)
        {
            int curDotIndex = dotIndexes[i];
            int nextDotIndex = dotIndexes[i + 1];

            // If the next index is consecutive then add it to the result
            if (curDotIndex + 1 == nextDotIndex)
            {
                // Make sure we have our current index first
                if (!result.Contains(curDotIndex))
                    result.Add(curDotIndex);

                // Add the next index
                result.Add(nextDotIndex);

                // If our result matches the length return it
                if (result.Count == length)
                    return result;
            }
            else
            {
                result.Clear();
            }
        }

        result.Clear();
        return result;
    }
}