using System.Linq;

public class CharacterExpressionUseCase
{
    private readonly IExpressionDataGateway _expressionDataGateway;

    public CharacterExpressionUseCase(IExpressionDataGateway expressionDataGateway)
    {
        _expressionDataGateway = expressionDataGateway;
    }

    public string GetSpriteNameForEmotion(string characterName, string emotion)
    {
        ExpressionData expressionData = _expressionDataGateway.GetExpressionData(characterName);

        if (expressionData == null)
        {
            return null;
        }

        // 해당 감정에 대한 스프라이트 탐색
        var emotionEntry = expressionData.Expressions.FirstOrDefault(e =>
            e.Emotion.Equals(emotion, System.StringComparison.OrdinalIgnoreCase));

        if (emotionEntry != null && emotionEntry.SpriteNames != null && emotionEntry.SpriteNames.Count > 0)
        {
            // 랜덤 스프라이트 이름 반환
            return emotionEntry.SpriteNames[new System.Random().Next(0, emotionEntry.SpriteNames.Count)];
        }

        // 해당 감정에 대한 스프라이트가 없을 경우 'idle' 표정 반환
        var idleEntry = expressionData.Expressions.FirstOrDefault(e =>
            e.Emotion.Equals("idle", System.StringComparison.OrdinalIgnoreCase));

        if (idleEntry != null && idleEntry.SpriteNames != null && idleEntry.SpriteNames.Count > 0)
        {
            return idleEntry.SpriteNames[new System.Random().Next(0, idleEntry.SpriteNames.Count)];
        }

        return null;
    }
}