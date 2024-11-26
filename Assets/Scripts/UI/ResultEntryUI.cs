using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultEntryUI : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PosText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI WpmText;

    public void Init(string name, string pos, string time, string wpm)
    {
        NameText.text = name;
        PosText.text = pos;
        TimeText.text = time;
        WpmText.text = wpm;
    }
}
