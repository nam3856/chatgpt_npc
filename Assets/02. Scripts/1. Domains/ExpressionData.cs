using System.Collections.Generic;

public class ExpressionData
{
    public string CharacterName { get; }
    public List<ExpressionEntryData> Expressions { get; }

    public ExpressionData(string characterName, List<ExpressionEntryData> expressions)
    {
        CharacterName = characterName;
        Expressions = expressions ?? new List<ExpressionEntryData>();
    }
}

public class ExpressionEntryData
{
    public string Emotion { get; }
    public List<string> SpriteNames { get; }

    public ExpressionEntryData(string emotion, List<string> spriteNames)
    {
        Emotion = emotion;
        SpriteNames = spriteNames ?? new List<string>();
    }
}