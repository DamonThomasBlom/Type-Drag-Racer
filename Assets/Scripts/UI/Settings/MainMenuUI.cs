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
    public UISlideIn StatsPanel;
    public UISlideIn LeaderboardPanel;
    public UISlideIn SettingsPanel;

    private void Awake()
    {
        PlayBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(PlayPanel, !PlayPanel.On)));
        CustomizeBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(CustomizePanel, !CustomizePanel.On)));
        StatsBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(StatsPanel, !StatsPanel.On)));
        LeaderboardBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(LeaderboardPanel, !LeaderboardPanel.On)));
        SettingsBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(SettingsPanel, !SettingsPanel.On)));

        // Close all panels
        CloseAllPanels(null);
    }

    void CloseAllPanels(Action callback)
    {
        Debug.Log("Close all panels");
        if (callback == null)
        {
            PlayPanel.SlideOut(() => PlayPanel.gameObject.SetActive(false));
            CustomizePanel.SlideOut(() => CustomizePanel.gameObject.SetActive(false));
            StatsPanel.SlideOut(() => StatsPanel.gameObject.SetActive(false));
            LeaderboardPanel.SlideOut(() => LeaderboardPanel.gameObject.SetActive(false));
            SettingsPanel.SlideOut(() => SettingsPanel.gameObject.SetActive(false));

            return;
        }

        PlayPanel.SlideOut(() => { callback.Invoke(); PlayPanel.gameObject.SetActive(false); });  
        CustomizePanel.SlideOut(() => { callback.Invoke(); CustomizePanel.gameObject.SetActive(false); });
        StatsPanel.SlideOut(() => { callback.Invoke(); StatsPanel.gameObject.SetActive(false); });
        LeaderboardPanel.SlideOut(() => { callback.Invoke(); LeaderboardPanel.gameObject.SetActive(false); });
        SettingsPanel.SlideOut(() => { callback.Invoke(); SettingsPanel.gameObject.SetActive(false); });
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
