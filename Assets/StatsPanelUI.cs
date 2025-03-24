using System;
using TMPro;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    public TextMeshProUGUI LeftSideText;
    public TextMeshProUGUI RightSideText;

    private void Start()
    {
        LeftSideText.text = string.Empty;
        RightSideText.text = string.Empty;

        foreach(var stat in StatsManager.Instance.playerStats.Stats)
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
}
