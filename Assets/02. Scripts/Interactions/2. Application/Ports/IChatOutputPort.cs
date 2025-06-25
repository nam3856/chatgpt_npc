public interface IChatOutputPort
{
    void DisplayUserMessage(string message);
    void DisplayAiMessage(string characterDisplayName, string replyMessage, string emotion);
    void DisplayErrorMessage(string errorMessage);
    void ShowLoadingIndicator();
    void HideLoadingIndicator();
    void ScrollToBottom();
}