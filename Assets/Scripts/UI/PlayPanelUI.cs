using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayPanelUI : MonoBehaviour
{
    public UISlideIn GameModesPanel;
    public UISlideIn GameSettingsPanel;

    public TMP_InputField PlayerNameInput;

    [Header("Play Panel")]
    public Button QuickPlayBtn;
    public Button SinglePlayerBtn;
    public Button MultiplayerBtn;
    public Button PracticeBtn;

    [Header("Game Settings Panel")]
    public Button BackBtn;
    public Button RaceBtn;

    private void Start()
    {
        QuickPlayBtn.onClick.AddListener(() => LoadScene(RaceGameMode.QuickPlay));
        SinglePlayerBtn.onClick.AddListener(() =>
        {
            RaceBtn.onClick.RemoveAllListeners();
            RaceBtn.onClick.AddListener(() => LoadScene(RaceGameMode.SinglePlayer));

            ShowPanel(GameSettingsPanel);
        });
        MultiplayerBtn.onClick.AddListener(() =>
        {
            RaceBtn.onClick.RemoveAllListeners();
            RaceBtn.onClick.AddListener(() => LoadScene(RaceGameMode.Multiplayer));

            ShowPanel(GameSettingsPanel);
        });
        PracticeBtn.onClick.AddListener(() => LoadScene(RaceGameMode.Practice));

        BackBtn.onClick.AddListener(() => ShowPanel(GameModesPanel));

        LoadPlayerName();
    }

    void LoadScene(RaceGameMode gameMode)
    {
        Player.Instance.GameMode = gameMode;

        switch(gameMode)
        {
            case RaceGameMode.QuickPlay:
            case RaceGameMode.Multiplayer:
            case RaceGameMode.SinglePlayer:
                CustomSceneManager.Instance.LoadScene("Multiplayer");
                break;

            case RaceGameMode.Practice:
                break;
        }
    }

    void ShowPanel(UISlideIn panel)
    {
        if (panel == GameModesPanel)
        {
            GameSettingsPanel.SlideOut(() =>
            {
                GameSettingsPanel.gameObject.SetActive(false);
                GameModesPanel.gameObject.SetActive(true);
                GameModesPanel.SlideIn();
            });
        }
        else if (panel == GameSettingsPanel)
        {
            GameModesPanel.SlideOut(() =>
            {
                GameModesPanel.gameObject.SetActive(false);
                GameSettingsPanel.gameObject.SetActive(true);
                GameSettingsPanel.SlideIn();
            });
        }
    }

    // Lists of adjectives and nouns
    private readonly List<string> adjectives = new List<string>
    {
        "Swift", "Brave", "Mighty", "Clever", "Fearless",
        "Sneaky", "Loyal", "Charming", "Witty", "Bold",
        "Fierce", "Silent", "Gallant", "Dynamic", "Ethereal"
    };

    private readonly List<string> nouns = new List<string>
    {
        "Warrior", "Fox", "Dragon", "Knight", "Hunter",
        "Mage", "Ranger", "Phantom", "Shadow", "Beast",
        "Titan", "Wizard", "Samurai", "Guardian", "Wolf"
    };

    private const string PLAYER_NAME_KEY = "PlayerName";

    public string GenerateRandomName()
    {
        string adjective = adjectives[UnityEngine.Random.Range(0, adjectives.Count)];
        string noun = nouns[UnityEngine.Random.Range(0, nouns.Count)];
        return $"{adjective} {noun}";
    }

    void LoadPlayerName()
    {
        //Player.Instance.PlayerName = PlayerPrefs.GetString(PLAYER_NAME_KEY, GenerateRandomName());
        PlayerNameInput.text = Player.Instance.PlayerName;

        PlayerNameInput.onValueChanged.AddListener((value) => SavePlayerName(value));
    }

    private void SavePlayerName(string value)
    {
        if (string.IsNullOrEmpty(value)) { return; }

        // TODO: Implement updating player name in database
        //Player.Instance.PlayerName = value;
        PlayerPrefs.SetString(PLAYER_NAME_KEY, value);
        PlayerPrefs.Save();
    }
}
