using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AdventOfCodeDay6 : MonoBehaviour
{
    public TextAsset input;

    const char Up = '^';
    const char Down = 'v';
    const char Left = '<';
    const char Right = '>';

    enum Direction { up, down, left, right };

    Direction TurnRight(Direction currentDirection)
    {
        switch (currentDirection)
        {
            case Direction.up: return Direction.right;
            case Direction.right: return Direction.down;
            case Direction.down: return Direction.left;
            case Direction.left: return Direction.up;
        }

        Debug.LogError("Faulty direction");
        return Direction.up;
    }

    KeyValuePair<int, int> GetDirectionCords(KeyValuePair<int, int> curCord, Direction currentDirection)
    {
        switch(currentDirection)
        {
            case Direction.up:
                return new KeyValuePair<int, int>(curCord.Key - 1, curCord.Value);

            case Direction.right:
                return new KeyValuePair<int, int>(curCord.Key, curCord.Value + 1);

            case Direction.down:
                return new KeyValuePair<int, int>(curCord.Key + 1, curCord.Value);

            case Direction.left:
                return new KeyValuePair<int, int>(curCord.Key, curCord.Value - 1);
        }

        Debug.LogError("Faulty cords");
        return new KeyValuePair<int, int>(-1, -1);
    }

    [Button]
    public void SolvePartOne()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] map = new char[row, col];

        var startingDirection = Direction.up;
        var startingCords = new KeyValuePair<int, int>(0, 0);

        // Initialize our grid values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                map[i, j] = lines[i][j];
                switch(map[i, j])
                {
                    case Left:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.left;
                        break;
                    case Right:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.right;
                        break;
                    case Up:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.up;
                        break;
                    case Down:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.down;
                        break;
                }
            }
        }

        DebugMatrix(map);
        Debug.Log("Starting cords = " + startingCords.ToString());
        Debug.Log("Starting direction = " + startingDirection.ToString());

        MoveGaurd(ref map, startingCords, startingDirection);

        Debug.Log("Recursion loop finish");
        DebugMatrix(map);

        int sumOfX = 0;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (map[i, j] == 'X')
                    sumOfX++;
            }
        }

        Debug.Log("Sum of X's - " +  sumOfX.ToString());
    }

    void MoveGaurd(ref char[,] map, KeyValuePair<int, int> curPos, Direction direction)
    {
        // Loop the maxing length of our map
        for(int i = 0; i < math.max(map.GetLength(0), map.GetLength(1)); i++)
        {
            // First and foremost mark our current position with an 'X'
            map[curPos.Key, curPos.Value] = 'X';

            // Check if we have left bounds
            KeyValuePair<int, int> nextPos = GetDirectionCords(curPos, direction);
            if (nextPos.Key < 0 || nextPos.Key >= map.GetLength(0) || nextPos.Value < 0 || nextPos.Value >= map.GetLength(1))
            {
                Debug.Log("We have left the map");
                return;
            }

            // We ran into obstalce, turn 90 degrees
            if (map[nextPos.Key, nextPos.Value] == '#')
            {
                MoveGaurd(ref map, curPos, TurnRight(direction));
                return;
            }

            curPos = nextPos;
        }
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        //string[] lines = input.text.Split("\n");
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] map = new char[row, col];

        var startingDirection = Direction.up;
        var startingCords = new KeyValuePair<int, int>(0, 0);

        // Initialize our grid values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                map[i, j] = lines[i][j];
                switch (map[i, j])
                {
                    case Left:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.left;
                        break;
                    case Right:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.right;
                        break;
                    case Up:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.up;
                        break;
                    case Down:
                        startingCords = new KeyValuePair<int, int>(i, j);
                        startingDirection = Direction.down;
                        break;
                }
            }
        }

        List<KeyValuePair<int, int>> candidates = new List<KeyValuePair<int, int>>();

        // Identify candidate positions for obstruction
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (map[i, j] == '.' && !(i == startingCords.Key && j == startingCords.Value))
                {
                    candidates.Add(new KeyValuePair<int, int>(i, j));
                }
            }
        }

        int validPositions = 0;

        // Test each candidate position
        foreach (var candidate in candidates)
        {
            // Create a temporary map with the obstruction
            char[,] tempMap = (char[,])map.Clone();
            tempMap[candidate.Key, candidate.Value] = '#';

            // Simulate guard movement
            if (GuardGetsStuckInLoop(tempMap, startingCords, startingDirection))
            {
                validPositions++;
            }
        }

        Debug.Log("Number of valid positions for obstruction: " + validPositions);
    }

    bool GuardGetsStuckInLoop(char[,] map, KeyValuePair<int, int> start, Direction direction)
    {
        HashSet<(int, int, Direction)> visited = new HashSet<(int, int, Direction)>();
        var curPos = start;

        while (true)
        {
            // Mark the current state
            if (!visited.Add((curPos.Key, curPos.Value, direction)))
            {
                // If we've visited this state before, we are in a loop
                return true;
            }

            // Calculate the next position
            var nextPos = GetDirectionCords(curPos, direction);

            // Check bounds
            if (nextPos.Key < 0 || nextPos.Key >= map.GetLength(0) || nextPos.Value < 0 || nextPos.Value >= map.GetLength(1))
            {
                return false; // Guard leaves the map
            }

            // Check for obstacles
            if (map[nextPos.Key, nextPos.Value] == '#')
            {
                direction = TurnRight(direction); // Turn right
            }
            else
            {
                curPos = nextPos; // Move forward
            }
        }
    }

    public static void DebugMatrix<T>(T[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        string output = "Matrix Debug Output:\n";

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                output += matrix[i, j] + "\t"; // Add tabs for spacing
            }
            output += "\n"; // New line after each row
        }

        Debug.Log(output);
    }
}