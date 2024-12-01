using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdventOfCodeDay1 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void Sovle()
    {
        List<int> LeftList = new List<int>();
        List<int> RightList = new List<int>();
        string[] newInput = input.text.Split('\n');

        Debug.Log("Line count = " + newInput.Length);

        foreach (var line in newInput)
        {
            LeftList.Add(int.Parse(line.Split("   ")[0]));
            RightList.Add(int.Parse(line.Split("   ")[1]));
        }

        LeftList.Sort();
        RightList.Sort();

        int totalOfDifferences = 0;

        for (int i = 0; i < LeftList.Count; i++)
        {
            totalOfDifferences += Mathf.Abs(LeftList[i] - RightList[i]);
        }

        Debug.Log("Total Differences = " +  totalOfDifferences);
    }

    [Button]
    public void SovlePartTwo()
    {
        List<int> LeftList = new List<int>();
        List<int> RightList = new List<int>();
        string[] newInput = input.text.Split('\n');

        Debug.Log("Line count = " + newInput.Length);

        foreach (var line in newInput)
        {
            LeftList.Add(int.Parse(line.Split("   ")[0]));
            RightList.Add(int.Parse(line.Split("   ")[1]));
        }

        LeftList.Sort();
        RightList.Sort();

        int totalOfSimilarities = 0;

        for (int i = 0; i < LeftList.Count; i++)
        {
            int currentNumber = LeftList[i];
            totalOfSimilarities += currentNumber * RightList.Count(n => n == currentNumber);
        }

        Debug.Log("Total Similarities = " + totalOfSimilarities);
    }
}
