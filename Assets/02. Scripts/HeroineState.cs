using System.Collections.Generic;

public enum MoodType
{
    Neutral,
    Happy,
    Sad,
    Angry,
    Nervous,
    Excited,
    Tired,
    Embarrassed
}

public enum AffinityLevel { Stranger, Acquaintance, Friend, Crush, Love }

public class HeroineState
{
    public string Name { get; }
    public int Affection { get; set; }
    public int InteractionCount { get; set; }
    public MoodType Mood { get; set; }
    public AffinityLevel AffinityLevel => CalculateAffinityLevel();
    public HashSet<string> FlagStates { get; } = new();
    public HashSet<string> UnlockedEvents { get; } = new();

    public HeroineState(string name)
    {
        Name = name;
        Mood = MoodType.Neutral;
        Affection = 30; // 기본값
    }

    private AffinityLevel CalculateAffinityLevel()
    {
        if (Affection >= 80) return AffinityLevel.Love;
        if (Affection >= 60) return AffinityLevel.Crush;
        if (Affection >= 40) return AffinityLevel.Friend;
        if (Affection >= 20) return AffinityLevel.Acquaintance;
        return AffinityLevel.Stranger;
    }
}
