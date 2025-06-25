using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Characters/Base Character")]
public class BaseCharacterSO : ScriptableObject
{
    public string characterName;
    public string characterDisplayName;
    public string position;

    [TextArea(3, 10)]
    public string gptPrompt;

    [TextArea(3, 10)]
    public string personalitySummary;

    // 경기 능력치
    [Header("Game Stats")]
    [Range(0, 100)] public int batting;
    [Range(0, 100)] public int pitching;
    [Range(0, 100)] public int defense;
    [Range(0, 100)] public int speed;
    [Range(0, 100)] public int stamina;
    [Range(0, 100)] public int gameSense;

    [Header("Support Stats")]
    [Range(0, 100)] public int leadership;
    [Range(0, 100)] public int strategy;
    [Range(0, 100)] public int motivation;
    [Range(0, 100)] public int focus;
    [Range(0, 100)] public int catchSense;
}
