using UnityEngine;
public class CharacterData
{
    public string Name { get; }
    public string DisplayName { get; }
    public EPosition Position { get; }
    public string GptPrompt { get; }
    public string PersonalitySummary { get; }

    public MoodType CurrentMood { get; private set; } = MoodType.Neutral;

    public int InteractionCount { get; private set; } = 0;
    public int LikabilityScore { get; private set; } = 30;

    public HeroineGameStats GameStats { get; }

    public CharacterData(
        string name, string displayName, EPosition position, string gptPrompt, string personalitySummary,
        HeroineGameStats gameStats)
    {
        Name = name;
        DisplayName = displayName;
        Position = position;
        GptPrompt = gptPrompt;
        PersonalitySummary = personalitySummary;
        GameStats = gameStats;
    }

    public void SetMood(MoodType mood)
    {
        CurrentMood = mood;
    }

    public void ResetMood()
    {
        CurrentMood = MoodType.Neutral;
    }
    public void IncrementInteractionCount() => InteractionCount++;

    public void UpdateLikabilityScore(int delta)
    {
        if (delta < -5 || delta > 5)
            throw new System.ArgumentOutOfRangeException(nameof(delta), "Delta must be between -5 and 5.");
        LikabilityScore = Mathf.Clamp(LikabilityScore + delta, 0, 100);
    }
}