using Newtonsoft.Json;
using UnityEngine;

public class WordsJson : MonoBehaviour
{
    [JsonProperty("words")]
    public string[] Words { get; set; }
}
