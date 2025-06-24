using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Characters/Base Character")]
public class BaseCharacterSO : ScriptableObject
{
    public string characterName;
    public string characterDisplayName;
    public string position;
    [TextArea(3, 10)]
    public string gptPrompt;
}
