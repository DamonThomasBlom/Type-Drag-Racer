using System.Collections;
using System.Collections.Generic;
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
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo)
            playerTransform = playerGo.transform;
        else
            Invoke(nameof(AssignPlayerTransform), 0.1f);
    }

    #endregion

    public string username;
    public string carName;
    public Transform playerTransform;
}
