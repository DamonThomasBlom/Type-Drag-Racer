using System;
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
    public UISlideIn CustomizePanel;
    public UISlideIn SettingsPanel;

    private void Awake()
    {
        StatsBtn.interactable = false;
        LeaderboardBtn.interactable = false;

        PlayBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(PlayPanel, !PlayPanel.On)));
        CustomizeBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(CustomizePanel, !CustomizePanel.On)));
        SettingsBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(SettingsPanel, !SettingsPanel.On)));
    }

    void CloseAllPanels(Action callback)
    {
        Debug.Log("Close all panels");
        if (callback == null)
        {
            PlayPanel.SlideOut();
            CustomizePanel.SlideOut();
            SettingsPanel.SlideOut();

            return;
        }
        
        PlayPanel.SlideOut(callback);
        CustomizePanel.SlideOut(callback);
        SettingsPanel.SlideOut(callback);
    }

    void TogglePanel(UISlideIn panel, bool enabled)
    {
        Debug.Log($"{panel.name} panel: {enabled}");

        if (enabled)
        {
            panel.gameObject.SetActive(true);
            panel.SlideIn();
        }
        else
        {
            panel.SlideOut(() => panel.gameObject.SetActive(false));
        }
    }
}
