using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public struct LeaderboardEntry
{
    public float FinishTime;
    public string Name;
    public int FinishPosition;
    public int WPM;
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

    public TextMeshProUGUI FinishText;
    public Transform LeaderboardContentHolder;
    public ResultEntryUI ResultEntryPrefab;

    int leaderBoardPosition = 1;
    float firstPlaceTime;

    public void AddEntry(LeaderboardEntry entry)
    {
        entry.FinishPosition = leaderBoardPosition;
        Leaders.Add(entry);

        leaderBoardPosition++;

        if (entry.Name == Player.Instance.PlayerName)
        {
            // Enable and set the finishText with a message
            if (FinishText != null)
            {
                FinishText.gameObject.SetActive(true);
                FinishText.text = GetFinishMessage(entry.FinishPosition);
            }
        }

        InstantiateResultEntry(entry);
    }

    private void InstantiateResultEntry(LeaderboardEntry leaderboardEntry)
    {
        ResultEntryUI result = Instantiate(ResultEntryPrefab, LeaderboardContentHolder);

        Debug.Log($"{leaderboardEntry.FinishTime} - {NetworkGameManager.Instance.RaceStartTimeNetwork} = {leaderboardEntry.FinishTime - NetworkGameManager.Instance.RaceStartTimeNetwork}");

        // Minus from the start time of the race
        leaderboardEntry.FinishTime -= NetworkGameManager.Instance.RaceStartTimeNetwork;

        // Record the first-place time
        if (leaderboardEntry.FinishPosition == 1)
        {
            firstPlaceTime = leaderboardEntry.FinishTime;
        }

        // Format the finish time depending on the position
        string finishTimeText;
        if (leaderboardEntry.FinishPosition == 1)
        {
            // First place shows exact time
            finishTimeText = leaderboardEntry.FinishTime.ToString("F2");
        }
        else
        {
            // Other positions show the time difference from first place
            float timeDifference = leaderboardEntry.FinishTime - firstPlaceTime;
            finishTimeText = $"+{timeDifference:F2}";
        }

        // Initialize the result UI
        result.Init(
            leaderboardEntry.Name,
            GetPosition(leaderboardEntry.FinishPosition),
            finishTimeText,
            leaderboardEntry.WPM.ToString()
        );
    }

    private string GetPosition(int position)
    {
        // Generate a friendly message based on the finish position
        switch (position)
        {
            case 1:
                return "1st";
            case 2:
                return "2nd";
            case 3:
                return "3rd";
            default:
                return $"{position}th";
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
