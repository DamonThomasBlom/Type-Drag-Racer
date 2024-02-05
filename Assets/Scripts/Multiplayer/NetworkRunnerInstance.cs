using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRunnerInstance : MonoBehaviour
{
    public static NetworkRunnerInstance Instance;

    private void Awake()
    {
        // Ensure there's only one instance
        if (Instance != null && Instance != this)
        {
            // Destroy the component itself
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public NetworkRunner Runner
    {
        get
        {
            return NetworkRunner.GetRunnerForGameObject(this.gameObject);
        }
    }
}
