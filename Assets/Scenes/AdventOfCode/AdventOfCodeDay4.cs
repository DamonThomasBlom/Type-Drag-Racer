using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AdventOfCodeDay4 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SovlePartOne()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] grid = new char[row, col];

        // Initialize our grid values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                grid[i, j] = lines[i][j];
            }
        }

        DebugMatrix(grid);

        int totalXmasCount = 0;

        // Now we want to check how many times XMAS appears in any order in our list
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grid[i, j] == 'X')
                    totalXmasCount += CheckNeighboursForWord(grid, row, col, i, j);
            }
        }

        Debug.Log("Total Xmas Count: " + totalXmasCount);
    }

    int CheckNeighboursForWord(char[,] grid, int row, int col, int curRow, int curCol)
    {
        //Debug.Log("Check neighbours");
        // Since arrays start from 0 index
        int maxWordLength = 4; // Adjusted for "XMAS" being 4 letters

        int totalXmasCount = 0;

        // Directions to check: (rowOffset, colOffset)
        var directions = new (int, int)[]
        {
        (0, 1),  // Right
        (0, -1), // Left
        (1, 0),  // Down
        (-1, 0), // Up
        (1, 1),  // Down-Right
        (-1, -1), // Up-Left
        (-1, 1), // Up-Right
        (1, -1) // Down-Left
        };

        foreach (var (rowOffset, colOffset) in directions)
        {
            string word = GetWordInDirection(grid, curRow, curCol, colOffset, rowOffset, maxWordLength);
            if (word == "XMAS")
            {
                totalXmasCount++;
            }
        }

        return totalXmasCount;
    }

    string GetWordInDirection(char[,] grid, int startRow, int startCol, int horizontalShift, int verticalShift, int wordLength)
    {
        string word = string.Empty;

        for (int i = 0; i < wordLength; i++)
        {
            int newRow = startRow + (verticalShift * i);
            int newCol = startCol + (horizontalShift * i);

            // Safety check to ensure indices are in bounds
            if (newRow < 0 || newRow >= grid.GetLength(0) || newCol < 0 || newCol >= grid.GetLength(1))
                return string.Empty; // Exit early if out of bounds

            word += grid[newRow, newCol];
        }

        //Debug.Log("Final word = " + word);

        return word;
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] grid = new char[row, col];

        // Initialize our grid values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                grid[i, j] = lines[i][j];
            }
        }

        DebugMatrix(grid);

        int totalXmasCount = 0;

        // Now we want to check how many times XMAS appears in any order in our list
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grid[i, j] == 'A')
                    totalXmasCount += CheckNeighboursForWordPartTwo(grid, i, j);
            }
        }

        Debug.Log("Total Xmas Count: " + totalXmasCount);
    }

    int CheckNeighboursForWordPartTwo(char[,] grid, int row, int col)
    {
        string diagonalOne = GetCharAtAxis(grid, row + 1, col + 1).ToString() + 'A'.ToString() + GetCharAtAxis(grid, row - 1, col - 1).ToString();
        string diagonalTwo = GetCharAtAxis(grid, row - 1, col + 1).ToString() + 'A'.ToString() + GetCharAtAxis(grid, row + 1, col - 1).ToString();

        bool passedCheckOne = false;
        bool passedCheckTwo = false;
        if (diagonalOne == "MAS" || diagonalOne == "SAM")
            passedCheckOne = true;

        if (diagonalTwo == "MAS" || diagonalTwo == "SAM")
            passedCheckTwo = true;

        if (passedCheckOne && passedCheckTwo)
            return 1;

        return 0;
    }

    char GetCharAtAxis(char[,] grid, int row, int col)
    {
        // Safety check to ensure indices are in bounds
        if (row < 0 || row >= grid.GetLength(0) || col < 0 || col >= grid.GetLength(1))
            return ' '; // Exit early if out of bounds

        return grid[row, col];
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
