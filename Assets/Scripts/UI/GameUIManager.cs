using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject ResultsPanel;
    public GameObject GameFinishPanel;

    public Button LeaderBoardBtn;
    public Button CloseLeaderBoardBtn;

    public Button PlayAgainBtn;
    public Button BackToMenuBtn;

    private void Start()
    {
        if (LeaderBoardBtn)
        {
            LeaderBoardBtn.onClick.AddListener(() => {
                ResultsPanel.SetActive(true);
                GameFinishPanel.SetActive(false);
            });
        }

        if (CloseLeaderBoardBtn)
        {
            CloseLeaderBoardBtn.onClick.AddListener(() => {
                ResultsPanel.SetActive(false);
                GameFinishPanel.SetActive(true);
            });
        }

        if (PlayAgainBtn)
        {
            PlayAgainBtn.onClick.AddListener(() => CustomSceneManager.Instance.LoadScene("Multiplayer"));
        }

        if (BackToMenuBtn)
        {
            BackToMenuBtn.onClick.AddListener(() => CustomSceneManager.Instance.LoadScene("MainMenu"));
        }
    }
}
