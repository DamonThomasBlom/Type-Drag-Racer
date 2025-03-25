using Fusion;
using System;
using TMPro;
using UnityEngine;

public class RemotePlayer : NetworkBehaviour, ICarSpeed
{
    public GameStatistics stats;
    public float speed { get; set; }

    [Networked, OnChangedRender(nameof(SpawnCar))]
    public string carPrefabName { get; set; }

    [Networked, OnChangedRender(nameof(UpdateAIMaterial)), Capacity(50)]
    public string carMaterialName { get; set; }

    [Networked, OnChangedRender(nameof(UpdateAIWheels)), Capacity(50)]
    public string carWheelName { get; set; }

    [Networked, OnChangedRender(nameof(SetPlayerName))]
    public string playerName { get; set; }

    [Networked, OnChangedRender(nameof(SetPlayerSpeed))]
    public float networkedSpeed { get; set; }

    public bool isLocalPlayer => Object && Object.HasStateAuthority;

    public Transform carSpawnPoint;

    public TextMeshProUGUI nameText;

    private Transform _localPlayerTransform;
    private PlayerController _localPlayerController;
    private GameObject _carInstance;
    private CustomizeCar _customizeCar;
    private AudioSource _carAudio;

    public override void Spawned()
    {
        base.Spawned();
        if (isLocalPlayer)
        {
            playerName = Player.Instance.PlayerName;
            carPrefabName = Player.Instance.CarName;
            carMaterialName = Player.Instance.MaterialName;
            carWheelName = Player.Instance.WheelName;
            _localPlayerTransform = Player.Instance.PlayerTransform;
            _localPlayerController = _localPlayerTransform.GetComponent<PlayerController>();
            carSpawnPoint.gameObject.SetActive(false);
            _carAudio = GetComponent<AudioSource>();

            _carAudio.enabled = false;
            if (nameText != null)
                nameText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            SpawnCar();
            SetPlayerName();
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (isLocalPlayer && _localPlayerTransform)
        {
            ApplyLocalData();
        }
    }

    private void SpawnCar()
    {
        if (string.IsNullOrEmpty(carPrefabName)) { return; }
        if (_carInstance != null) { return; }

        var selectedCarPrefab = PrefabManager.Instance.GetCarPrefab(carPrefabName);
        _carInstance = Instantiate(selectedCarPrefab, carSpawnPoint.position, carSpawnPoint.rotation, carSpawnPoint);
        _customizeCar = _carInstance.GetComponent<CustomizeCar>();

        UpdateAIMaterial();
        UpdateAIWheels();
    }

    void UpdateAIMaterial()
    {
        if (!string.IsNullOrEmpty(carMaterialName) && _customizeCar != null)
        {
            _customizeCar.UpdateMaterials(PrefabManager.Instance.GetMaterialByName(carMaterialName));
        }
    }

    void UpdateAIWheels()
    {
        if (!string.IsNullOrEmpty(carWheelName) && _customizeCar != null)
        {
            _customizeCar.UpdateWheels(PrefabManager.Instance.GetWheelScriptable(carWheelName));
        }
    }

    private void SetPlayerName()
    {
        if (nameText != null)
        {
            nameText.text = playerName;
            nameText.gameObject.SetActive(Player.Instance.InGameSettings.ShowPlayerNames);
        }
    }

    private void SetPlayerSpeed()
    {
        speed = networkedSpeed;
    }

    protected virtual void ApplyLocalData()
    {
        transform.position = _localPlayerTransform.position;
        stats = _localPlayerController.gameStatistics;
        speed = _localPlayerController.speed;
        networkedSpeed = speed;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void FinishedRaceRpc(float time, string name, int wordsPerMinute)
    {
        RaceLeaderboardManager.Instance.AddEntry(new LeaderboardEntry() { FinishTime = time, Name = name, WPM = wordsPerMinute });
    }
}
