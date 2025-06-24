public interface IChatOutputPort
{
    void DisplayUserMessage(string message);
    void DisplayAiMessage(string characterName, string replyMessage, string emotion);
    void DisplayErrorMessage(string errorMessage);
    void ShowLoadingIndicator();
    void HideLoadingIndicator();
    void ScrollToBottom();
}