using Fusion;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float currentSpeed;
    public float distanceTraveled;
    public GameStatistics gameStatistics { get; set; }

    private Vector3 startPosition; 

    public void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        gameStatistics = TypingManager.Instance.GameStats;
        currentSpeed = TypingManager.Instance.GameStats.wordsPerMinute / TypingManager.Instance.fastestWPM * TypingManager.Instance.fastestCarSpeed;

        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        GameUIManager.Instance.speedTxt.text = "km/h: " + currentSpeed.ToString("F0");
        GameUIManager.Instance.distanceTraveledTxt.text = "Distance Traveled: " + distanceTraveled.ToString("F0") + "m";

        // Move the player forward based on the current speed
        transform.Translate(Vector3.forward * currentSpeed * GameManager.Instance.conversionFactor * Time.deltaTime);
    }
}
