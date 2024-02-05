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
    public float speed { get; set; }

    public Transform carSpawnPoint;

    private Vector3 startPosition; 

    public void Start()
    {
        ResetStartPosition();
        SpawnCar();
    }

    private void Update()
    {
        gameStatistics = TypingManager.Instance.GameStats;
        speed = TypingManager.Instance.GameStats.wordsPerMinute / TypingManager.Instance.fastestWPM * TypingManager.Instance.fastestCarSpeed;

        distanceTraveled = Vector3.Distance(startPosition, transform.position);

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
