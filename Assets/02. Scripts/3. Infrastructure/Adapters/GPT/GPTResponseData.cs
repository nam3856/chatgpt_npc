public class GPTResponseData
{
    public string ReplyMessage { get; }
    public string Emotion { get; }

    public GPTResponseData(string replyMessage, string emotion)
    {
        ReplyMessage = replyMessage;
        Emotion = emotion;
    }
}