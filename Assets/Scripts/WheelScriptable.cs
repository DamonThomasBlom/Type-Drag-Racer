using UnityEngine;

[CreateAssetMenu(fileName = "WheelScriptable", menuName = "ScriptableObjects/WheelScriptable")]
public class WheelScriptable : ScriptableObject
{
    public string Name;
    public GameObject LeftWheel;
    public GameObject RightWheel;
}
