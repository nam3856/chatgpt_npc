using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Characters/Base Character")]
public class BaseCharacterSO : ScriptableObject
{
    public string characterName;
    public string position;
    public Sprite portrait;
    [TextArea(3, 10)]
    public string gptPrompt;
}
