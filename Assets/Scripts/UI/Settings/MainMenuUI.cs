using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button PlayBtn;
    public Button CustomizeBtn;
    public Button StatsBtn;
    public Button LeaderboardBtn;
    public Button SettingsBtn;
    public Button ExitBtn;

    private void Awake()
    {
        CustomizeBtn.interactable = false;
        StatsBtn.interactable = false;
        LeaderboardBtn.interactable = false;
        SettingsBtn.interactable = false;
    }
}
