using Newtonsoft.Json;
using OpenAI.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class SendChatUseCase
{
    private readonly IChatGPTGateway _chatGPTGateway;
    private readonly IChatOutputPort _chatOutputPort;
    private readonly ICharacterImageOutputPort _characterImageOutputPort; // 추가된 의존성
    private readonly List<ChatMessage> _conversationHistory;
    private readonly CharacterData _characterData;
    private readonly CharacterData _ruleData;
    public CharacterData GetCharacterData() => _characterData;

    public SendChatUseCase(
        IChatGPTGateway chatGPTGateway,
        IChatOutputPort chatOutputPort,
        CharacterData characterData,
        CharacterData ruleData,
        List<ChatMessage> conversationHistory,
        ICharacterImageOutputPort characterImageOutputPort) // 생성자 매개변수 추가
    {
        _chatGPTGateway = chatGPTGateway;
        _chatOutputPort = chatOutputPort;
        _characterData = characterData;
        _ruleData = ruleData;
        _conversationHistory = conversationHistory ?? new List<ChatMessage>();
        _characterImageOutputPort = characterImageOutputPort; // 할당

        if (_conversationHistory.Count == 0 && !string.IsNullOrWhiteSpace(_characterData.GptPrompt))
        {
            string systemPrompt = $@"
{_ruleData.GptPrompt}
{_characterData.GptPrompt}
";
            _conversationHistory.Add(new ChatMessage(EMessageRole.System, systemPrompt));
            Debug.Log(systemPrompt);
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

        _conversationHistory.Add(new ChatMessage(EMessageRole.User, $@"Current Interaction Count: {_characterData.InteractionCount}
Current Likability Score: {_characterData.LikabilityScore}
Current Emotion: {_characterData.CurrentMood}"+userMessage));

        _chatOutputPort.DisplayUserMessage("나: " + userMessage);

        _chatOutputPort.ShowLoadingIndicator();

        try
        {
            GPTResponseData response = await _chatGPTGateway.GetChatCompletionAsync(_conversationHistory, _characterData, _ruleData);

            if (response != null && !string.IsNullOrEmpty(response.ReplyMessage) && _characterData != null)
            {
                _conversationHistory.Add(new ChatMessage(EMessageRole.Assistant, response.ReplyMessage, response.Emotion));
                _chatOutputPort.DisplayAiMessage(_characterData.DisplayName, response.ReplyMessage, response.Emotion);
                _characterData.SetMood(ParseEmotion(response.Emotion));

                _characterData.IncrementInteractionCount();
                // 캐릭터 표정 업데이트
                if (_characterImageOutputPort != null)
                {
                    await _characterImageOutputPort.UpdateCharacterImage(_characterData.Name, response.Emotion);
                }
            }
            else
            {
                _chatOutputPort.DisplayErrorMessage("GPT 응답 없음.");
            }
        }
        catch (JsonException jsonEx)
        {
            Debug.LogWarning("JSON 파싱 실패: " + jsonEx.Message);
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

    private MoodType ParseEmotion(string emotion)
    {
        return emotion.ToLower() switch
        {
            "happy" => MoodType.Happy,
            "sad" => MoodType.Sad,
            "angry" => MoodType.Angry,
            "nervous" => MoodType.Nervous,
            "excited" => MoodType.Excited,
            "tired" => MoodType.Tired,
            "embarrassed" => MoodType.Embarrassed,
            "flirting" => MoodType.Happy,
            "playful" => MoodType.Happy,
            _ => MoodType.Neutral
        };
    }
}