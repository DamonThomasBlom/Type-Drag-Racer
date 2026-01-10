using UnityEngine;

public class DisableOnAwake : MonoBehaviour
{
    public float delay = 0;

    private void Awake()
    {
        if (delay > 0)
        {
            Invoke(nameof(Disable), delay);
            return;
        }
        gameObject.SetActive(false);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
