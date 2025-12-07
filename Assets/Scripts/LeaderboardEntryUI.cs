using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour
{
    [Header("Colours")]
    public Color Gold;
    public Color Silver;
    public Color Bronze;
    public Color Normal;

    [Header("UI References")]
    public Image RankImage;
    public TextMeshProUGUI RankTxt;
    public TextMeshProUGUI UsernameTxt;
    public TextMeshProUGUI DistanceTxt;
    public TextMeshProUGUI AccuracyTxt;
    public TextMeshProUGUI TimeTxt;
    public TextMeshProUGUI WpmTxt;
    public GameObject YourRanking;

    public void Init(int rank, string username, string distance, float accuracy, float time, float wpm)
    {
        // Set text values
        RankTxt.text = rank.ToString();
        UsernameTxt.text = username;
        DistanceTxt.text = distance;
        
        //AccuracyTxt.text = $"{accuracy:F1}%"; // Format accuracy to 2 decimal places
        AccuracyTxt.text = $"{Math.Round(accuracy, 1)}%"; // Format accuracy to 2 decimal places
        //TimeTxt.text = $"{time:F2}"; // Format time to 2 decimal places
        TimeTxt.text = $"{Math.Round(time, 2)}"; // Format time to 2 decimal places
        WpmTxt.text = $"{wpm:F0}"; // Format WPM to 0 decimal place

        // Assign rank colors
        switch (rank)
        {
            case 1:
                RankImage.color = Gold;
                break;
            case 2:
                RankImage.color = Silver;
                break;
            case 3:
                RankImage.color = Bronze;
                break;
            default:
                RankImage.color = Normal;
                break;
        }
    }

    public void SetYourRanking(bool yourRanking)
    {
        YourRanking.SetActive(yourRanking);
    }
}
