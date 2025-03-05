using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Fusion.Sockets.NetBitBuffer;
using Debug = UnityEngine.Debug;

public class AdventOfCodeDay12 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SolvePartOne()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("lines count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] map = new char[row, col];

        // Initialize our grid values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                map[i, j] = lines[i][j];
            }
        }

        // Store all our visiting spots
        bool[,] visited = new bool[row, col];

        int totalFenceCost = 0;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                var areaAndPerimeter = CalculateAreaAndPerimeter(map[i, j], i, j, map, ref visited);
                totalFenceCost += areaAndPerimeter.Item1 * areaAndPerimeter.Item2;

                if (areaAndPerimeter.Item1 != 0 && areaAndPerimeter.Item2 != 0)
                    Debug.Log($"[{i},{j}] ({map[i, j]}) Area = {areaAndPerimeter.Item1} | Perimeter = {areaAndPerimeter.Item2}");
            }
        }

        Debug.Log("Total = " + totalFenceCost);
    }

    Tuple<int, int> CalculateAreaAndPerimeter(char currentPlant, int x, int y, char[,] map, ref bool[,] visited)
    {
        int area = 0;
        int perimeter = 0;

        // First value is the area and second value is the perimeter
        Tuple<int, int> AreaAndPerimeter = new Tuple<int, int>(area, perimeter);

        // Check bounds
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1)) 
            return AreaAndPerimeter;

        // Have we visited this already
        if (visited[x, y]) 
            return AreaAndPerimeter;

        var directions = new (int, int)[]
        {
        (0, 1),  // Right
        (0, -1), // Left
        (1, 0),  // Down
        (-1, 0), // Up
        };

        // This is our highest perimeter possible, with every neighbour we find we subtract one perimeter
        perimeter = 4;
        area = 1;

        AreaAndPerimeter = new Tuple<int, int>(0, 0);
        visited[x, y] = true;

        foreach (var (xOffset, yOffset) in directions)
        {
            int newX = x + xOffset;
            int newY = y + yOffset;

            if (isValidNeighbour(currentPlant, newX, newY, map))
            {
                perimeter--;
                var neightbourAreaAndPerimeter = CalculateAreaAndPerimeter(currentPlant, newX, newY, map, ref visited);
                int item1 = AreaAndPerimeter.Item1 + neightbourAreaAndPerimeter.Item1;
                int item2 = AreaAndPerimeter.Item2 + neightbourAreaAndPerimeter.Item2;
                AreaAndPerimeter = new Tuple<int, int>(item1, item2);
            }
        }

        int finalItem1 = AreaAndPerimeter.Item1 + area;
        int finalItem2 = AreaAndPerimeter.Item2 + perimeter;
        AreaAndPerimeter = new Tuple<int, int>(finalItem1, finalItem2);

        return AreaAndPerimeter;
    }

    bool isValidNeighbour(char currentPlant, int x, int y, char[,] map)
    {
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
            return false;

        if (map[x, y] == currentPlant)
            return true;

        return false;
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("lines count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] map = new char[row, col];

        // Initialize our grid values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                map[i, j] = lines[i][j];
            }
        }

        // Store all our visiting spots
        bool[,] visited = new bool[row, col];

        int totalFenceCost = 0;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                var areaAndSides = CalculateAreaAndPerimeterPartTwo(map[i, j], i, j, map, new List<Sides>(), ref visited);
                totalFenceCost += areaAndSides.Item1 * areaAndSides.Item2;

                if (areaAndSides.Item1 != 0 && areaAndSides.Item2 != 0)
                    Debug.Log($"[{i},{j}] ({map[i, j]}) Area = {areaAndSides.Item1} | Sides = {areaAndSides.Item2}");
            }
        }

        Debug.Log("Total = " + totalFenceCost);
    }

    public enum Sides
    {
        top, bottom, left, right
    }

    Tuple<int, int> CalculateAreaAndPerimeterPartTwo(char currentPlant, int x, int y, char[,] map, List<Sides> previousSides, ref bool[,] visited)
    {
        int area = 0;
        int sides = 0;

        // First value is the area and second value is the perimeter
        Tuple<int, int> AreaAndPerimeter = new Tuple<int, int>(area, sides);

        // Check bounds
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
            return AreaAndPerimeter;

        // Have we visited this already
        if (visited[x, y])
            return AreaAndPerimeter;

        var directions = new (int, int)[]
        {
        (0, 1),  // Right
        (0, -1), // Left
        (1, 0),  // Down
        (-1, 0), // Up
        };

        // This is our highest perimeter possible, with every neighbour we find we subtract one perimeter
        area = 1;

        AreaAndPerimeter = new Tuple<int, int>(0, 0);
        visited[x, y] = true;

        List<Sides> ourSides = new List<Sides>();

        foreach (var (xOffset, yOffset) in directions)
        {
            int newX = x + xOffset;
            int newY = y + yOffset;

            if (!isValidNeighbour(currentPlant, newX, newY, map))
            {
                Sides side = OffsetToSide(xOffset, yOffset);
                Debug.Log("Adding to our sides - " + side);
                ourSides.Add(side);
            }
        }

        foreach (var (xOffset, yOffset) in directions)
        {
            int newX = x + xOffset;
            int newY = y + yOffset;

            if (isValidNeighbour(currentPlant, newX, newY, map))
            {
                sides--;
                var neightbourAreaAndPerimeter = CalculateAreaAndPerimeterPartTwo(currentPlant, newX, newY, map, ourSides, ref visited);
                int item1 = AreaAndPerimeter.Item1 + neightbourAreaAndPerimeter.Item1;
                int item2 = AreaAndPerimeter.Item2 + neightbourAreaAndPerimeter.Item2;
                AreaAndPerimeter = new Tuple<int, int>(item1, item2);
            }
        }

        foreach(var side in previousSides)
        {
            ourSides.Remove(side);
        }

        int finalItem1 = AreaAndPerimeter.Item1 + area;
        int finalItem2 = AreaAndPerimeter.Item2 + ourSides.Count;
        AreaAndPerimeter = new Tuple<int, int>(finalItem1, finalItem2);

        return AreaAndPerimeter;
    }

    Sides OffsetToSide(int x, int y)
    {
        if (x == 0)
        {
            if (y == 1)
                return Sides.right;
            else if (y == -1)
                return Sides.left;
        }

        if (y == 0)
        {
            if (x == 1)
                return Sides.bottom;
            if (x == -1)
                return Sides.top;
        }

        Debug.LogError("Error side");
        return Sides.top;
    }

    bool isValidNeighbourPartTwo(char currentPlant, int x, int y, char[,] map)
    {
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
            return false;

        if (map[x, y] == currentPlant)
            return true;

        return false;
    }
}