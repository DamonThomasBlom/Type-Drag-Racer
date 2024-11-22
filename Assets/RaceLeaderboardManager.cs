using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct LeaderboardEntry
{
    public float FinishTime;
    public string Name;
    public int FinishPosition;
}

public class RaceLeaderboardManager : MonoBehaviour
{
    #region SINGLETON

    public static RaceLeaderboardManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [ShowInInspector]
    public List<LeaderboardEntry> Leaders = new List<LeaderboardEntry>();

    public TextMeshProUGUI finishText;

    int leaderBoardPosition = 1;
    public void AddEntry(LeaderboardEntry entry)
    {
        entry.FinishPosition = leaderBoardPosition;
        Leaders.Add(entry);
        leaderBoardPosition++;

        if (entry.Name == Player.Instance.username)
        {
            // Enable and set the finishText with a message
            if (finishText != null)
            {
                finishText.gameObject.SetActive(true);
                finishText.text = GetFinishMessage(entry.FinishPosition);
            }
        }
    }

    private string GetFinishMessage(int position)
    {
        // Generate a friendly message based on the finish position
        switch (position)
        {
            case 1:
                return "You Finished 1st!";
            case 2:
                return "You Finished 2nd!";
            case 3:
                return "You Finished 3rd!";
            default:
                return $"You Finished {position}th!";
        }
    }
}
