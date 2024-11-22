using Sirenix.OdinInspector;
using UnityEngine;

public interface ICarSpeed
{
    [ShowInInspector]
    float speed { get; set; }
}

public class PlayerController : MonoBehaviour, ICarSpeed
{
    public float distanceTraveled;
    public GameStatistics gameStatistics { get; set; }
    [ShowInInspector]
    public float speed { get; set; }

    public Transform carSpawnPoint;

    private Vector3 startPosition;
    private bool _raceFinished = false;

    public void Start()
    {
        ResetStartPosition();
        SpawnCar();
    }

    private void Update()
    {
        if (TypingManager.Instance == null) { return; }

        gameStatistics = TypingManager.Instance.GameStats;
        speed = TypingManager.Instance.GameStats.wordsPerMinute / TypingManager.Instance.fastestWPM * TypingManager.Instance.fastestCarSpeed;

        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled >= GameManager.Instance.RaceDistance && !_raceFinished)
        {
            Debug.Log("Race finsihed");
            _raceFinished = true;
            Player.Instance.localPlayerInstance.FinishedRaceRpc(NetworkGameManager.Instance.ElapsedNetworkTime, Player.Instance.username);
        }

        GameUIManager.Instance.speedTxt.text = "km/h: " + speed.ToString("F0");
        GameUIManager.Instance.distanceTraveledTxt.text = "Distance Traveled: " + distanceTraveled.ToString("F0") + "m";

        // Move the player forward based on the current speed
        transform.Translate(Vector3.forward * speed * GameManager.Instance.conversionFactor * Time.deltaTime);
    }

    public void ResetStartPosition()
    {
        startPosition = transform.position;
    }

    void SpawnCar()
    {
        var selectedCarPrefab = PrefabManager.Instance.GetCarPrefab(Player.Instance.carName);
        Instantiate(selectedCarPrefab, carSpawnPoint.position, carSpawnPoint.rotation, carSpawnPoint);
    }
}
