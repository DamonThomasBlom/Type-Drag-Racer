using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using System.Linq;
using Michsky.MUIP;
using UnityEngine.Events;

public class LeaderboardPanelUI : MonoBehaviour
{
    public Transform LeaderBoardHolder;
    //public TMP_Dropdown RaceDistanceDropdown;
    public CustomDropdown RaceDistanceDropdown;
    public LeaderboardEntryUI LeaderboardEntry;

    UISlideIn _uiSlideIn;

    void Start()
    {
        _uiSlideIn = GetComponent<UISlideIn>();
        //InitializeDropdowns();
        InitializeDropdown(RaceDistanceDropdown, typeof(RaceDistance));
        FetchLeaderboard();
    }

    //void InitializeDropdowns()
    //{
    //    RaceDistanceDropdown.ClearOptions();

    //    // Create a new list with "Any" as the first option
    //    List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>
    //    {
    //        new TMP_Dropdown.OptionData("Any")
    //    };

    //    // Add enum values to the list
    //    options.AddRange(
    //        Enum.GetNames(typeof(RaceDistance))
    //        .Select(name => new TMP_Dropdown.OptionData(name))
    //    );

    //    RaceDistanceDropdown.AddOptions(options);
    //    RaceDistanceDropdown.value = 0;
    //    RaceDistanceDropdown.RefreshShownValue();

    //    RaceDistanceDropdown.onValueChanged.AddListener(delegate { FetchLeaderboard(); });
    //}

    void InitializeDropdown(CustomDropdown dropdown, Type enumType)
    {
        dropdown.items.Clear();

        var anyItem = new CustomDropdown.Item
        {
            itemName = "Any",
            OnItemSelection = new UnityEvent()
        };
        dropdown.items.Add(anyItem);

        foreach (string name in Enum.GetNames(enumType))
        {
            var item = new CustomDropdown.Item
            {
                itemName = name,
                OnItemSelection = new UnityEvent()
            };

            dropdown.items.Add(item);
        }

        dropdown.SetupDropdown();
    }

    void FetchLeaderboard()
    {
        ClearLeaderboard();

        string selectedRace = RaceDistanceDropdown.items[RaceDistanceDropdown.selectedItemIndex].itemName;
        //string selectedRace = RaceDistanceDropdown.options[RaceDistanceDropdown.value].text;

        if (selectedRace == "Any")
        {
            DatabaseManager.Instance.GetTop10LeaderBoard(DisplayLeaderboard);
        }
        else
        {
            RaceDistance raceDistance = (RaceDistance)Enum.Parse(typeof(RaceDistance), selectedRace);
            DatabaseManager.Instance.GetTop10LeaderBoardByRace(raceDistance, DisplayLeaderboard);
        }
    }

    void DisplayLeaderboard(LeaderBoardResponse response)
    {
        bool localPlayerFound = false;

        for (int i = 0; i < response.Items.Count; i++)
        {
            var entry = response.Items[i];
            var newEntry = Instantiate(LeaderboardEntry, LeaderBoardHolder);
            newEntry.Init(i + 1, entry.Username, entry.RaceDistance, entry.Accuracy, entry.Time, entry.Wpm);

            if (entry.Username == Player.Instance.PlayerName)
            {
                newEntry.SetYourRanking(true);
                localPlayerFound = true;
            }
        }

        if (!localPlayerFound)
        {
            FetchLocalPlayerRank();
        }
    }

    void FetchLocalPlayerRank()
    {
        string selectedRace = RaceDistanceDropdown.items[RaceDistanceDropdown.selectedItemIndex].itemName;
        //string selectedRace = RaceDistanceDropdown.options[RaceDistanceDropdown.value].text;

        if (selectedRace == "Any")
        {
            DatabaseManager.Instance.GetPlayerBestWPM(response =>
            {
                if (response.Items.Count > 0)
                {
                    int bestWpm = (int)response.Items[0].Wpm;
                    playerDatabaseEntry = response.Items[0];
                    DatabaseManager.Instance.GetPlayerGlobalRank(bestWpm, GetLocalPlayerEntry);
                }
            });
        }
        else
        {
            RaceDistance raceDistance = (RaceDistance)Enum.Parse(typeof(RaceDistance), selectedRace);
            DatabaseManager.Instance.GetPlayerBestWPMForRace(raceDistance, response =>
            {
                if (response.Items.Count > 0)
                {
                    int bestWpm = (int)response.Items[0].Wpm;
                    playerDatabaseEntry = response.Items[0];
                    DatabaseManager.Instance.GetPlayerRankForRace(bestWpm, raceDistance, GetLocalPlayerEntry);
                }
            });
        }
    }

    LeaderboardDatabaseItem playerDatabaseEntry;

    void GetLocalPlayerEntry(LeaderBoardResponse response)
    {
        if (response.Items.Count > 0)
        {
            var entry = response.Items[0];
            var newEntry = Instantiate(LeaderboardEntry, LeaderBoardHolder);
            newEntry.Init((int)response.TotalItems, Player.Instance.PlayerName, playerDatabaseEntry.RaceDistance, playerDatabaseEntry.Accuracy, playerDatabaseEntry.Time, playerDatabaseEntry.Wpm);
            newEntry.SetYourRanking(true);
        }
    }

    void ClearLeaderboard()
    {
        foreach (Transform child in LeaderBoardHolder)
        {
            Destroy(child.gameObject);
        }
    }
}
