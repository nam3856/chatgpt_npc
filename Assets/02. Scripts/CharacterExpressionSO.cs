using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterExpression", menuName = "Character/Expression")]
public class CharacterExpressionSO : ScriptableObject
{
    public string characterName; // 캐릭터 이름 or ID
    public List<ExpressionEntry> expressions;

    [System.Serializable]
    public class ExpressionEntry
    {
        public string emotion; // 예: "excited", "sad", "smile"
        public Sprite[] sprites; // 여러 장 중 랜덤
    }

    public Sprite GetRandomSpriteForEmotion(string emotion)
    {
        foreach (var entry in expressions)
        {
            if (entry.emotion.Equals(emotion, System.StringComparison.OrdinalIgnoreCase))
            {
                if (entry.sprites != null && entry.sprites.Length > 0)
                {
                    return entry.sprites[Random.Range(0, entry.sprites.Length)];
                }
            }
        }
        Debug.LogWarning($"[표정 없음] {characterName}에게 {emotion} 감정 스프라이트가 없습니다.");

        // 해당 감정에 대한 스프라이트가 없을 경우 idle 표정 반환
        foreach (var entry in expressions)
        {
            if (entry.emotion.Equals("idle", System.StringComparison.OrdinalIgnoreCase) && entry.sprites != null && entry.sprites.Length > 0)
            {
                return entry.sprites[Random.Range(0, entry.sprites.Length)];
            }
        }
        return null;
    }
}
