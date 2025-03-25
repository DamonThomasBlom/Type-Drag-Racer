using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour
{
    public TextMeshProUGUI LeftSideText;
    public TextMeshProUGUI RightSideText;
    public Button ResetStatsBtn;

    private CustomGraphChartFeed _chartFeed;

    private void Start()
    {
        RenderStats();
        ResetStatsBtn.onClick.AddListener(ResetStats);
        _chartFeed = GetComponentInChildren<CustomGraphChartFeed>();
    }

    private void RenderStats()
    {
        LeftSideText.text = string.Empty;
        RightSideText.text = string.Empty;

        foreach (var stat in StatsManager.Instance.playerStats.Stats)
        {
            LeftSideText.text += stat.Key + "\n";

            if (float.TryParse(stat.Value, out float value))
            {
                RightSideText.text += Math.Round(value, 1).ToString();
                if (stat.Key.ToLower().Contains("accuracy"))
                    RightSideText.text += "%";

                RightSideText.text += "\n";
            }
            else
            {
                RightSideText.text += stat.Value + "\n";
            }
        }
    }

    void ResetStats()
    {
        StatsManager.Instance.ResetStats();
        RenderStats();
        _chartFeed.LoadGraph();
    }
}
