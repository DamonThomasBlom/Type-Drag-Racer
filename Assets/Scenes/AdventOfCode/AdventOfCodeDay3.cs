using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class AdventOfCodeDay3 : MonoBehaviour
{
    public TextAsset input;

    [Button]
    public void SovlePartOne()
    {
        string[] newInput = input.text.Split('\n');
        Debug.Log("Line count = " + newInput.Length);

        string singleLineInput = string.Empty;

        foreach (var line in newInput)
        {
            singleLineInput += line.Replace("\n", "");
        }

        var test = singleLineInput.AllIndexesOf("mul(");

        int totalMul = 0;

        foreach(var value in test)
        {
            //Debug.Log("Indexes are " + value);
            totalMul += MultiplyIfValid(value, singleLineInput);
        }

        Debug.Log("Total: " + totalMul);
    }

    int MultiplyIfValid(int indexToLookAt, string input)
    {
        int returnValue = 0;

        // This is the shortest possible sequence
        if (indexToLookAt > input.Length - 8)
            return returnValue;

        int indexOfOpeningBracket = input.IndexOf("(", indexToLookAt);
        int indexOfClosingBracket = input.IndexOf(")", indexToLookAt);

        string twoNumbers = input.Substring(indexOfOpeningBracket + 1, indexOfClosingBracket - indexOfOpeningBracket - 1);

        string[] numbersArr = twoNumbers.Split(",");
        if (numbersArr.Length > 2)           // More then two commas
            return returnValue;
        if (twoNumbers.Contains(" "))        // White spaces
            return returnValue;
        int firstNum = 0;
        int secondNum = 0;
        if (int.TryParse(numbersArr[0], out firstNum) && int.TryParse(numbersArr[1], out secondNum))   // If its a valid parse then multiple them
        {
            returnValue = firstNum * secondNum;
        }

        Debug.Log("Return value - " + returnValue);

        return returnValue;
    }

    [Button]
    public void SolvePartTwo()
    {
        string[] newInput = input.text.Split('\n');
        Debug.Log("Line count = " + newInput.Length);

        string singleLineInput = string.Empty;

        foreach (var line in newInput)
        {
            singleLineInput += line.Replace("\n", "");
        }

        var test = singleLineInput.AllIndexesOf("mul(");

        int totalMul = 0;

        foreach (var value in test)
        {
            //Debug.Log("Indexes are " + value);
            totalMul += MultiplyIfValidPartTwo(value, singleLineInput);
        }

        Debug.Log("Total: " + totalMul);
    }

    int MultiplyIfValidPartTwo(int indexToLookAt, string input)
    {
        int returnValue = 0;

        // This is the shortest possible sequence
        if (indexToLookAt > input.Length - 8)
            return returnValue;

        int lastDo = input.Substring(0, indexToLookAt).LastIndexOf("do()");   
        int lastDont = input.Substring(0, indexToLookAt).LastIndexOf("don't()");
        Debug.Log($"Last do() {lastDo} last dont {lastDont}");
        if (lastDont > lastDo)  // If the most recent dont is greater then do then return
            return returnValue;

        int indexOfOpeningBracket = input.IndexOf("(", indexToLookAt);
        int indexOfClosingBracket = input.IndexOf(")", indexToLookAt);

        string twoNumbers = input.Substring(indexOfOpeningBracket + 1, indexOfClosingBracket - indexOfOpeningBracket - 1);

        string[] numbersArr = twoNumbers.Split(",");
        if (numbersArr.Length > 2)           // More then two commas
            return returnValue;
        if (twoNumbers.Contains(" "))        // White spaces
            return returnValue;
        int firstNum = 0;
        int secondNum = 0;
        if (int.TryParse(numbersArr[0], out firstNum) && int.TryParse(numbersArr[1], out secondNum))   // If its a valid parse then multiple them
        {
            returnValue = firstNum * secondNum;
        }

        return returnValue;
    }
}
