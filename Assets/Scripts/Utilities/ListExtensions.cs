using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void DebugList<T>(this List<T> list)
    {
        string output = string.Empty;

        foreach (T item in list)
        {
            output += item + ",";
        }

        Debug.Log(output);
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

    public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        T item = list[oldIndex];
        list.RemoveAt(oldIndex);
        list.Insert(newIndex, item);
    }

    public static void Swap<T>(this List<T> list, int index1, int index2)
    {
        T temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }
}
