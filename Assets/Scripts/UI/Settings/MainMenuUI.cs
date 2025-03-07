using JetBrains.Annotations;
using System;
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

    public UISlideIn PlayPanel;
    public UISlideIn SettingsPanel;
    public UISlideIn CustomizePanel;

    private void Awake()
    {
        StatsBtn.interactable = false;
        LeaderboardBtn.interactable = false;
        //SettingsBtn.interactable = false;

        PlayBtn.onClick.AddListener(() => CloseAllPanels(() => EnablePlayPanel(!PlayPanel.On)));
        CustomizeBtn.onClick.AddListener(() => CloseAllPanels(() => EnableCustomizePanel(!CustomizePanel.On)));
        SettingsBtn.onClick.AddListener(() => CloseAllPanels(() => EnableSettingsPanel(!SettingsPanel.On)));
    }

    void CloseAllPanels(Action callback)
    {
        Debug.Log("Close all panels");
        if (callback == null)
        {
            PlayPanel.SlideOut();
            SettingsPanel.SlideOut();

            return;
        }
        
        PlayPanel.SlideOut(callback);
        SettingsPanel.SlideOut(callback);
    }

    void EnablePlayPanel(bool enabled)
    {
        Debug.Log("Play panel: " + enabled);

        if (enabled)
        {
            PlayPanel.gameObject.SetActive(true);
            PlayPanel.SlideIn();
        }
        else
        {
            PlayPanel.SlideOut(() => PlayPanel.gameObject.SetActive(false));
        }
    }

    void EnableCustomizePanel(bool enabled)
    {
        Debug.Log("Customize panel: " + enabled);

        if (enabled)
        {
            CustomizePanel.gameObject.SetActive(true);
            CustomizePanel.SlideIn();
        }
        else
        {
            CustomizePanel.SlideOut(() => CustomizePanel.gameObject.SetActive(false));    
        }
    }

    void EnableSettingsPanel(bool enabled)
    {
        Debug.Log("Settings panel: " + enabled);

        if (enabled)
        {
            SettingsPanel.gameObject.SetActive(true);
            SettingsPanel.SlideIn();
        }
        else
        {
            SettingsPanel.SlideOut(() => SettingsPanel.gameObject.SetActive(false));
        }
    }
}
