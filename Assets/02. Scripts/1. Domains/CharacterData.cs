public class CharacterData
{
    public string Name { get; }
    public string Position { get; }
    public string GptPrompt { get; }

    public CharacterData(string name, string position, string gptPrompt)
    {
        Name = name;
        Position = position;
        GptPrompt = gptPrompt;
    }
}