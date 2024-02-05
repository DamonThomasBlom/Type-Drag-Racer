using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    #region SINGLETON
    public static GameUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public TextMeshProUGUI statsTxt;
    public TextMeshProUGUI speedTxt;
    public TextMeshProUGUI distanceTraveledTxt;
}
