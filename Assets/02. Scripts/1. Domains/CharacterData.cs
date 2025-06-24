public class CharacterData
{
    public string Name { get; }
    public string DisplayName { get; }
    public string Position { get; }
    public string GptPrompt { get; }

    public CharacterData(string name, string displayName, string position, string gptPrompt)
    {
        Name = name;
        DisplayName = displayName;
        Position = position;
        GptPrompt = gptPrompt;
    }
}