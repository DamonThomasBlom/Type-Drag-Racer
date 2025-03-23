using TMPro;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    public TextMeshProUGUI LeftSideText;
    public TextMeshProUGUI RightSideText;

    private void Start()
    {
        foreach(var stat in StatsManager.Instance.playerStats.Stats)
        {
            LeftSideText.text += stat.Key + "\n";
            RightSideText.text += stat.Value + "\n";
        }
    }
}
