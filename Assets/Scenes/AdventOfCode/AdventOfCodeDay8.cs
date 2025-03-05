using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AdventOfCodeDay8 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SolvePartOne()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] map = new char[row, col];
        char[,] antinodes = new char[row, col];

        // Initialize our map values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                antinodes[i, j] = '.';
                map[i, j] = lines[i][j];
            }
        }

        // Storing antenna frequency and location
        Dictionary<char, List<KeyValuePair<int, int>>> Antennas = new();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (map[i,j] != '.')
                {
                    char antennaFreq = map[i, j];
                    // Create antinode
                    if (Antennas.ContainsKey(antennaFreq))
                    {
                        Antennas[antennaFreq].Add(new KeyValuePair<int, int>(i, j));
                        CreateAntinodes(ref antinodes, Antennas[antennaFreq]);
                    }
                    else // Add it
                    {
                        List<KeyValuePair<int, int>> list = new() { new KeyValuePair<int, int>(i, j) };
                        Antennas.Add(antennaFreq, list);
                    }
                }
            }
        }

        Debug.Log("Antinodes");
        DebugMatrix(antinodes);

        int antinodeCount = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (antinodes[i, j] == '#')
                {
                    antinodeCount++;
                }
            }
        }

        Debug.Log("Antinode count: " + antinodeCount);
    }

    void CreateAntinodes(ref char[,] antinodes, List<KeyValuePair<int, int>> antennaLocations)
    {
        // The process I have figured out that works we need to calculate the difference of two cords
        // 2,3 : 4,6 = -2,-3
        // To put an antinode down we add the difference to the first cord and minus the difference from the second cord
        // and then we have our antinode positions
        // 2,3 + (-2,-3) = 0,0
        // 4,6 - (-2,-3) = 6,9 

        Debug.Log("Antennas Count: " + antennaLocations.Count);

        for(int i = 0; i < antennaLocations.Count; i++)
        {
            for (int j = 0; j < antennaLocations.Count; j++)
            {
                // Don't calculate on itself
                if (i == j) continue;

                CreateAntinode(ref antinodes, antennaLocations[i], antennaLocations[j]);
            }
        }
    }

    void CreateAntinode(ref char[,] antinodes, KeyValuePair<int, int> first, KeyValuePair<int, int> second)
    {
        int xDiff = first.Key - second.Key;
        int yDiff = first.Value - second.Value;

        var firstAntenna = new KeyValuePair<int, int>(first.Key + xDiff, first.Value + yDiff);
        var secondAntenna = new KeyValuePair<int, int>(second.Key - xDiff, second.Value - yDiff);

        // Make sure antenna isn't out of the map
        if (firstAntenna.Key > 0 && firstAntenna.Key < antinodes.GetLength(0)
            && firstAntenna.Value > 0 && firstAntenna.Value < antinodes.GetLength(1))
        {
            antinodes[firstAntenna.Key, firstAntenna.Value] = '#';
        }

        if (secondAntenna.Key >= 0 && secondAntenna.Key < antinodes.GetLength(0)
            && secondAntenna.Value >= 0 && secondAntenna.Value < antinodes.GetLength(1))
        {
            antinodes[secondAntenna.Key, secondAntenna.Value] = '#';
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

    [Button]
    public void SolvePartTwo()
    {
        string[] lines = input.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        //string[] lines = input.text.Split("\n");
        Debug.Log("Line count = " + lines.Length);

        int row = lines.Length;
        int col = lines[0].Length;

        char[,] map = new char[row, col];
        char[,] antinodes = new char[row, col];

        // Initialize our map values
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                antinodes[i, j] = '.';
                map[i, j] = lines[i][j];
            }
        }

        // Storing antenna frequency and location
        Dictionary<char, List<KeyValuePair<int, int>>> Antennas = new();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (map[i, j] != '.')
                {
                    char antennaFreq = map[i, j];
                    // Create antinode
                    if (Antennas.ContainsKey(antennaFreq))
                    {
                        Antennas[antennaFreq].Add(new KeyValuePair<int, int>(i, j));
                        CreateAntinodesPartTwo(ref antinodes, Antennas[antennaFreq]);
                    }
                    else // Add it
                    {
                        List<KeyValuePair<int, int>> list = new() { new KeyValuePair<int, int>(i, j) };
                        Antennas.Add(antennaFreq, list);
                    }
                }
            }
        }

        Debug.Log("Antinodes");
        DebugMatrix(antinodes);

        int antinodeCount = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (antinodes[i, j] == '#')
                {
                    antinodeCount++;
                }
            }
        }

        Debug.Log("Antinode count: " + antinodeCount);
    }

    void CreateAntinodesPartTwo(ref char[,] antinodes, List<KeyValuePair<int, int>> antennaLocations)
    {
        // The process I have figured out that works we need to calculate the difference of two cords
        // 2,3 : 4,6 = -2,-3
        // To put an antinode down we add the difference to the first cord and minus the difference from the second cord
        // and then we have our antinode positions
        // 2,3 + (-2,-3) = 0,0
        // 4,6 - (-2,-3) = 6,9 

        Debug.Log("Antennas Count: " + antennaLocations.Count);

        for (int i = 0; i < antennaLocations.Count; i++)
        {
            for (int j = 0; j < antennaLocations.Count; j++)
            {
                // Don't calculate on itself
                if (i == j) continue;

                CreateAntinodePartTwo(ref antinodes, antennaLocations[i], antennaLocations[j]);
            }
        }
    }

    void CreateAntinodePartTwo(ref char[,] antinodes, KeyValuePair<int, int> first, KeyValuePair<int, int> second)
    {
        int xDiff = first.Key - second.Key;
        int yDiff = first.Value - second.Value;

        var firstAntenna = new KeyValuePair<int, int>(first.Key, first.Value);

        bool firstAntinodeOutMap = false;
        while (!firstAntinodeOutMap)
        {
            firstAntenna = new KeyValuePair<int, int>(firstAntenna.Key + xDiff, firstAntenna.Value + yDiff);
            // Make sure antenna isn't out of the map
            if (firstAntenna.Key >= 0 && firstAntenna.Key < antinodes.GetLength(0)
                && firstAntenna.Value >= 0 && firstAntenna.Value < antinodes.GetLength(1))
            {
                antinodes[firstAntenna.Key, firstAntenna.Value] = '#';
            }
            else
            {
                firstAntinodeOutMap = true;
            }
        }

        var secondAntenna = new KeyValuePair<int, int>(second.Key, second.Value);
        bool secondAntinodeOutMap = false;

        while (!secondAntinodeOutMap)
        {
            secondAntenna = new KeyValuePair<int, int>(secondAntenna.Key + xDiff, secondAntenna.Value + yDiff);
            // Make sure antenna isn't out of the map
            if (secondAntenna.Key >= 0 && secondAntenna.Key < antinodes.GetLength(0)
                && secondAntenna.Value >= 0 && secondAntenna.Value < antinodes.GetLength(1))
            {
                antinodes[secondAntenna.Key, secondAntenna.Value] = '#';
            }
            else
            {
                secondAntinodeOutMap = true;
            }
        }
    }
}