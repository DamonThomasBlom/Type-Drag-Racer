using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region SINGLETON

    // The static instance of the class
    private static Player _instance;

    // Public property to access the instance
    public static Player Instance
    {
        get
        {
            // If the instance doesn't exist, create it
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("Player (Singleton)");
                _instance = singletonObject.AddComponent<Player>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        // Ensure there is only one instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            AssignPlayerTransform();
        }
    }

    void AssignPlayerTransform()
    {
        if (PlayerTransform != null) { return; }
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo)
            PlayerTransform = playerGo.transform;
        else
            Invoke(nameof(AssignPlayerTransform), 0.1f);
    }

    #endregion

    [ShowInInspector] public string PlayerName => PlayerData.Username;
    public RaceGameMode GameMode;
    public string CarName;
    public string MaterialName;
    public string WheelName;
    public Transform PlayerTransform;
    public RemotePlayer LocalPlayerInstance;
    [ShowInInspector] public GameSettings GameSettings = new GameSettings();
    public InGameSettings InGameSettings = new InGameSettings();

    [ShowInInspector] public UserData PlayerData { get; set; }
    public string Token;
}

public enum RaceGameMode { QuickPlay, SinglePlayer, Multiplayer, Practice }

public struct GameSettings
{
    public RacePlayerCount PlayerCount;
    public RaceDistance RaceDistance;
    public TypingDifficulty TypingDifficulty;
    public AIDifficulty AIDifficulty;
    public TrackEnvironment TrackEnvironment;
}

public struct InGameSettings
{
    public bool ShowPlayerNames { get; set; }
    public bool ShowTypingStats { get; set; }
    public bool ShowPing { get; set; }
    public bool ShowFPS { get; set; }
}
