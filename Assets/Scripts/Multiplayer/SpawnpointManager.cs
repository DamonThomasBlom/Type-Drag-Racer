using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Fusion;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnpointManager : SimulationBehaviour, IPlayerJoined
{
    public static SpawnpointManager Instance;

    public GameObject BotVeryEasyPrefab;
    public GameObject BotEasyPrefab;
    public GameObject BotMediumPrefab;
    public GameObject BotHardPrefab;
    public GameObject BotVeryHardPrefab;
    public GameObject BotExpertPrefab;

    [Header("Player")]
    public GameObject localPlayerPrefab;
    public GameObject remotePlayerPrefab;

    private NetworkPlayerSpawnPoint networkPlayerSpawnPoint;
    private Vector3 randomSpawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player joined");
        Debug.Log("Tick rate: " + Runner.TickRate);
        if (player == Runner.LocalPlayer)
        {
            SpawnLocalPlayer();
            //Invoke(nameof(SpawnRemotePlayer), 0.5f);

            if (Runner.IsSharedModeMasterClient)
            {
                //Invoke(nameof(GenerateBots), 10f);
            }
        }
        else
        {
            if (Runner.IsSharedModeMasterClient)
            {
                Vector3 spawnPoint = SerializedSpawnPoints.Instance.GetRandomPlayerSpawnPoint().position;
                networkPlayerSpawnPoint.AssignSpawnPointRpc(spawnPoint, player);
                networkPlayerSpawnPoint.UpdateSpawnPointsRpc(SerializedSpawnPoints.Instance.GetTakenSpawnPoints());
            }
        }

        //if (Runner.IsSharedModeMasterClient)
        //{
        //    if (player == Runner.LocalPlayer)
        //    {
        //        Vector3 spawnPoint = GetRandomPlayerSpawnPoint().position;
        //        SpawnLocalPlayer(spawnPoint, player);

        //        //GenerateBots();
        //        Invoke(nameof(GenerateBots), 10f);
        //    }
        //    else
        //    {
        //        Vector3 spawnPoint = GetRandomPlayerSpawnPoint().position;
        //        networkPlayerSpawnPoint.SpawnPlayerRpc(spawnPoint, player);
        //    }
        //}
    }

    public void SpawnLocalPlayer()
    {
        // Spawn local player
        randomSpawnPoint = SerializedSpawnPoints.Instance.GetRandomPlayerSpawnPoint().position;
        Player.Instance.PlayerTransform = Instantiate(localPlayerPrefab, randomSpawnPoint, Quaternion.identity).transform;

        SpawnRemotePlayer();
    }

    public void SpawnRemotePlayer()
    {
        Transform localPlayerTransform = Player.Instance.PlayerTransform;

        var runner = NetworkRunner.GetRunnerForGameObject(gameObject);
        NetworkObject networkObject = runner.Spawn(remotePlayerPrefab, localPlayerTransform.position, Quaternion.identity);
        networkPlayerSpawnPoint = networkObject.GetComponent<NetworkPlayerSpawnPoint>();
        Player.Instance.LocalPlayerInstance = networkObject.GetComponent<RemotePlayer>();
        Player.Instance.PlayerName += " - " + networkObject.Id.ToString();
        Player.Instance.LocalPlayerInstance.playerName = Player.Instance.PlayerName;
        networkObject.gameObject.name += " (Local)";
    }

    public void AssignSpawnPoint(Vector3 spawnPoint)
    {
        Transform localPlayerTransform = Player.Instance.PlayerTransform;
        localPlayerTransform.position = spawnPoint;

        // Reset the players start position
        FindObjectOfType<PlayerController>().ResetStartPosition();
    }

    [Button]
    public void GenerateBots()
    {
        // Adjust the likelihood of each bot type
        List<GameObject> botPrefabs = new List<GameObject>
        {
            BotVeryEasyPrefab, BotEasyPrefab, BotMediumPrefab,
            BotHardPrefab, BotVeryHardPrefab, BotExpertPrefab
        };

        // Get our bot spawn probabilities from our game manager
        List<float> botSpawnChances = GameManager.Instance.AIDifficulty.ToBotSpawnChances();

        // Create a separate list to store spawn points that need bots
        List<Transform> unoccupiedSpawnPoints = new List<Transform>();

        // Iterate over the spawn points to find unoccupied ones
        foreach (var spawnPoint in SerializedSpawnPoints.Instance.SpawnPoints)
        {
            if (!spawnPoint.Value)
            {
                unoccupiedSpawnPoints.Add(spawnPoint.Key);
            }
        }

        // Loop through each unoccupied spawn point
        foreach (var spawnPoint in unoccupiedSpawnPoints)
        {
            // Randomly choose a bot type based on probabilities
            int index = ChooseRandomIndex(botSpawnChances);
            GameObject selectedBotPrefab = botPrefabs[index];

            var runner = NetworkRunner.GetRunnerForGameObject(gameObject);

            // Instantiate the selected bot at the spawn point
            runner.Spawn(selectedBotPrefab, spawnPoint.position, Quaternion.identity);

            // Mark the spawn point as occupied
            SerializedSpawnPoints.Instance.SpawnPoints[spawnPoint] = true;
        }

        networkPlayerSpawnPoint.UpdateSpawnPointsRpc(SerializedSpawnPoints.Instance.GetTakenSpawnPoints());
    }

    private int ChooseRandomIndex(List<float> probabilities)
    {
        float total = 0;

        // Calculate the total probability
        foreach (var probability in probabilities)
        {
            total += probability;
        }

        // Generate a random value between 0 and the total probability
        float randomValue = Random.value * total;

        // Find the index associated with the chosen probability
        for (int i = 0; i < probabilities.Count; i++)
        {
            if (randomValue < probabilities[i])
            {
                return i;
            }

            randomValue -= probabilities[i];
        }

        // This should not happen, but return the last index as a fallback
        return probabilities.Count - 1;
    }
}
