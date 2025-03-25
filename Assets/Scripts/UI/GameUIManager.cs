using Fusion;
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

    [Header("UI References")]
    public TextMeshProUGUI typingStatsText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI pingText;

    [Header("FPS Calculation")]
    private int frameCount;
    private float elapsedTime;
    private float fps;

    [Header("Photon Fusion")]
    public NetworkRunner networkRunner;

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

        typingStatsText.gameObject.SetActive(Player.Instance.InGameSettings.ShowTypingStats);
        fpsText.gameObject.SetActive(Player.Instance.InGameSettings.ShowFPS);
        pingText.gameObject.SetActive(Player.Instance.InGameSettings.ShowPing);
    }

    void Update()
    {
        // Update FPS
        CalculateFPS();

        // Update Photon Fusion Ping
        if (networkRunner != null && networkRunner.IsRunning)
        {
            double ping = networkRunner.GetPlayerRtt(networkRunner.LocalPlayer);
            pingText.text = $"Ping: {ping * 1000:F0} ms";
        }
        else
        {
            pingText.text = "Ping: -- ms";
        }
    }

    void CalculateFPS()
    {
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= 1f) // Update FPS every second
        {
            fps = frameCount / elapsedTime;
            fpsText.text = $"FPS: {fps:F0}";
            frameCount = 0;
            elapsedTime = 0;
        }
    }
}
