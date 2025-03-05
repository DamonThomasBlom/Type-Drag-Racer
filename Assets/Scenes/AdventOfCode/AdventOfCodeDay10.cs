using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AdventOfCodeDay10 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SolvePartOne()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        int[,] map = new int[row, col];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                map[i, j] = int.Parse(lines[i][j].ToString());
            }
        }

        ListExtensions.DebugMatrix(map);

        int totalTrailheads = 0;

        // Loop through each element and start path finding from the 0s
        for (int i = 0;i < row; i++)
        {
            for(int j = 0;j < col; j++)
            {
                // Start path finding
                if (map[i, j] == 0)
                {
                    List<KeyValuePair<int, int>> HeightsRights = new List<KeyValuePair<int, int>>();
                    int trailheads = CalculateTrailHeadScores(0, i, j, map, ref HeightsRights);

                    //Debug.Log($"Start {i}:{j} has a total trail head of {trailheads}");
                    totalTrailheads += trailheads;
                }
            }
        }

        Debug.Log("Total Trailheads = " + totalTrailheads);
    }

    int CalculateTrailHeadScores(int currentHeight, int curX, int curY, int[,] map, ref List<KeyValuePair<int, int>> HeightsReached)
    {
        int totalTrailHeads = 0;

        int up = curX - 1;
        int down = curX + 1;
        int left = curY - 1;
        int right = curY + 1;

        // Up
        if (up >= 0 && !HeightsReached.Contains(new KeyValuePair<int, int>(up, curY)))
        {
            int nextHeight = map[up, curY];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                    HeightsReached.Add(new KeyValuePair<int, int>(up, curY));
                }
                else
                    totalTrailHeads += CalculateTrailHeadScores(nextHeight, up, curY, map, ref HeightsReached);
            }
        }

        // Down
        if (down < map.GetLength(0) && !HeightsReached.Contains(new KeyValuePair<int, int>(down, curY)))
        {
            int nextHeight = map[down, curY];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                    HeightsReached.Add(new KeyValuePair<int, int>(down, curY));
                }
                else
                    totalTrailHeads += CalculateTrailHeadScores(nextHeight, down, curY, map, ref HeightsReached);
            }
        }

        // Left
        if (left >= 0 && !HeightsReached.Contains(new KeyValuePair<int, int>(curX, left)))
        {
            int nextHeight = map[curX, left];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                    HeightsReached.Add(new KeyValuePair<int, int>(curX, left));
                }
                else
                    totalTrailHeads += CalculateTrailHeadScores(nextHeight, curX, left, map, ref HeightsReached);
            }
        }

        // Right
        if (right < map.GetLength(1) && !HeightsReached.Contains(new KeyValuePair<int, int>(curX, right)))
        {
            int nextHeight = map[curX, right];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                    HeightsReached.Add(new KeyValuePair<int, int>(curX, right));
                }
                else
                    totalTrailHeads += CalculateTrailHeadScores(nextHeight, curX, right, map, ref HeightsReached);
            }
        }

        //Debug.Log($"Visiting ({curX}, {curY}), Current Height: {currentHeight}, Total Trailheads: {totalTrailHeads}");
        return totalTrailHeads;
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        int[,] map = new int[row, col];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                map[i, j] = int.Parse(lines[i][j].ToString());
            }
        }

        ListExtensions.DebugMatrix(map);

        int totalTrailheads = 0;

        // Loop through each element and start path finding from the 0s
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                // Start path finding
                if (map[i, j] == 0)
                {
                    int trailheads = CalculateTrailHeadScoresPartTwo(0, i, j, map);

                    //Debug.Log($"Start {i}:{j} has a total trail head of {trailheads}");
                    totalTrailheads += trailheads;
                }
            }
        }

        Debug.Log("Total Trailheads = " + totalTrailheads);
    }

    int CalculateTrailHeadScoresPartTwo(int currentHeight, int curX, int curY, int[,] map)
    {
        int totalTrailHeads = 0;

        int up = curX - 1;
        int down = curX + 1;
        int left = curY - 1;
        int right = curY + 1;

        // Up
        if (up >= 0)
        {
            int nextHeight = map[up, curY];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                }
                else
                    totalTrailHeads += CalculateTrailHeadScoresPartTwo(nextHeight, up, curY, map);
            }
        }

        // Down
        if (down < map.GetLength(0))
        {
            int nextHeight = map[down, curY];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                }
                else
                    totalTrailHeads += CalculateTrailHeadScoresPartTwo(nextHeight, down, curY, map);
            }
        }

        // Left
        if (left >= 0)
        {
            int nextHeight = map[curX, left];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                }
                else
                    totalTrailHeads += CalculateTrailHeadScoresPartTwo(nextHeight, curX, left, map);
            }
        }

        // Right
        if (right < map.GetLength(1))
        {
            int nextHeight = map[curX, right];
            // Valid height trail
            if (nextHeight == currentHeight + 1)
            {
                if (nextHeight == 9)
                {
                    totalTrailHeads++;
                }
                else
                    totalTrailHeads += CalculateTrailHeadScoresPartTwo(nextHeight, curX, right, map);
            }
        }

        //Debug.Log($"Visiting ({curX}, {curY}), Current Height: {currentHeight}, Total Trailheads: {totalTrailHeads}");
        return totalTrailHeads;
    }
}