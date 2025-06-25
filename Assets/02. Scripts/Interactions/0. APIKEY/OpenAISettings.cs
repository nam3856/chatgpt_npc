using UnityEngine;

[CreateAssetMenu(fileName = "OpenAISettings", menuName = "Settings/OpenAI Settings")]
public class OpenAISettings : ScriptableObject
{
    [Header("OpenAI API Settings")]
    public string apiKey;
}
