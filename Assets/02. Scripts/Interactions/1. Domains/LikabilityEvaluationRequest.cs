public class LikabilityEvaluationRequest
{
    public string CharacterName { get; }
    public string PersonalitySummary { get; }
    public int AffectionScore { get; }
    public int InteractionCount { get; }
    public string PlayerMessage { get; }

    public LikabilityEvaluationRequest(string characterName, string personalitySummary, int affectionScore, int interactionCount, string playerMessage)
    {
        CharacterName = characterName;
        PersonalitySummary = personalitySummary;
        AffectionScore = affectionScore;
        InteractionCount = interactionCount;
        PlayerMessage = playerMessage;
    }
}