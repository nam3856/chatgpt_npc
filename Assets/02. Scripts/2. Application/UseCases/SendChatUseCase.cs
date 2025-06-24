using System.Collections.Generic;
using System.Threading.Tasks;

public class SendChatUseCase
{
    private readonly IChatGPTGateway _chatGPTGateway;
    private readonly IChatOutputPort _chatOutputPort;
    private readonly List<ChatMessage> _conversationHistory;
    private readonly CharacterData _characterData;
    private readonly CharacterData _ruleData;

    public SendChatUseCase(
        IChatGPTGateway chatGPTGateway,
        IChatOutputPort chatOutputPort,
        CharacterData characterData,
        CharacterData ruleData,
        List<ChatMessage> conversationHistory)
    {
        _chatGPTGateway = chatGPTGateway;
        _chatOutputPort = chatOutputPort;
        _characterData = characterData;
        _ruleData = ruleData;
        _conversationHistory = conversationHistory ?? new List<ChatMessage>();

        if (_conversationHistory.Count == 0 && !string.IsNullOrWhiteSpace(_characterData.GptPrompt))
        {
            string systemPrompt = $@"
You must always respond in the following JSON format:

{{
  ""ReplyMessage"": ""실제 응답 내용"",
  ""Emotion"": ""Emotions written in English (happy, sad, etc)""
}}

Do NOT use any emojis or markdown.
You must always respond in JSON format. Do not include extra commentary.
You must never use emojis (such as 😊, 😂, 😭, etc.) in any response. 
This rule is absolute. No exceptions are allowed. 
If you use emojis, the response is invalid.
{_ruleData.GptPrompt}
{_characterData.GptPrompt}
";
            _conversationHistory.Add(new ChatMessage(EMessageRole.System, systemPrompt));
        }
    }

    public async Task Execute(string userMessage)
    {
        if (string.IsNullOrEmpty(userMessage))
        {
            _chatOutputPort.DisplayErrorMessage("Error: Please enter a message.");
            return;
        }
        if (_characterData == null)
        {
            _chatOutputPort.DisplayErrorMessage("Error: Character data missing.");
            return;
        }

        _conversationHistory.Add(new ChatMessage(EMessageRole.User, userMessage));

        _chatOutputPort.DisplayUserMessage("나: " + userMessage);

        _chatOutputPort.ShowLoadingIndicator();

        try
        {
            GPTResponseData response = await _chatGPTGateway.GetChatCompletionAsync(_conversationHistory, _characterData, _ruleData);

            if (response != null && !string.IsNullOrEmpty(response.ReplyMessage))
            {
                _conversationHistory.Add(new ChatMessage(EMessageRole.Assistant, response.ReplyMessage, response.Emotion));
                _chatOutputPort.DisplayAiMessage(_characterData.Name, response.ReplyMessage, response.Emotion);
            }
            else
            {
                _chatOutputPort.DisplayErrorMessage("GPT 응답 없음.");
            }
        }
        catch (System.Exception e)
        {
            _chatOutputPort.DisplayErrorMessage("API 오류: " + e.Message);
        }
        finally
        {
            _chatOutputPort.HideLoadingIndicator();
            _chatOutputPort.ScrollToBottom();
        }
    }

    public IReadOnlyList<ChatMessage> GetConversationHistory()
    {
        return _conversationHistory.AsReadOnly();
    }
}