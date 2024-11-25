using System;
using System.Collections;
using UnityEngine;

public class DelayedActionUtility : MonoBehaviour
{
    private static DelayedActionUtility _instance;

    /// <summary>
    /// Singleton instance of the DelayedActionManager.
    /// </summary>
    public static DelayedActionUtility Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create a new GameObject and attach the manager to it
                GameObject managerObject = new GameObject("DelayedActionManager");
                _instance = managerObject.AddComponent<DelayedActionUtility>();
                DontDestroyOnLoad(managerObject); // Ensure it persists across scenes
            }
            return _instance;
        }
    }

    /// <summary>
    /// Executes an action after a specified delay.
    /// </summary>
    /// <param name="delay">The delay in seconds.</param>
    /// <param name="action">The action to execute.</param>
    public void PerformActionWithDelay(float delay, Action action)
    {
        if (action == null)
        {
            Debug.LogError("Action cannot be null!");
            return;
        }

        StartCoroutine(ExecuteActionAfterDelay(delay, action));
    }

    private IEnumerator ExecuteActionAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    private void OnDestroy()
    {
        // Ensure no lingering references to a destroyed instance
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
