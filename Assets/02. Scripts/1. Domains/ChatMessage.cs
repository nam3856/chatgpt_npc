public class ChatMessage
{
    public EMessageRole Role { get; }
    public string Content { get; }
    public string Emotion { get; }

    public ChatMessage(EMessageRole role, string content, string emotion = null)
    {
        Role = role;
        Content = content;
        Emotion = emotion;
    }
}