

using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Linq;


public class OpenAIChatGPTAdapter : IChatGPTGateway
{
    private readonly string _apiKey;

    public OpenAIChatGPTAdapter(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<GPTResponseData> GetChatCompletionAsync(List<ChatMessage> conversationHistory, CharacterData characterData, CharacterData ruleData)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new System.InvalidOperationException("API Key is not set in OpenAIChatGPTAdapter.");
        }
        if (conversationHistory == null || conversationHistory.Count == 0)
        {
            throw new System.ArgumentException("Conversation history cannot be null or empty.", nameof(conversationHistory));
        }

        var api = new OpenAIClient(_apiKey);

        // ChatMessage 엔티티를 OpenAI 라이브러리의 Message 타입으로 변환
        List<Message> openaiMessages = conversationHistory
            .Select(msg => new Message(ConvertRole(msg.Role), msg.Content))
            .ToList();

        var chatRequest = new ChatRequest(
            openaiMessages,
            Model.GPT4o
        );

        var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        var choice = response.FirstChoice;
        var aiMessageContent = choice?.Message?.Content as string;

        if (!string.IsNullOrEmpty(aiMessageContent))
        {
            string cleanedJson = aiMessageContent.Replace("```json", "").Replace("```", "").Trim();
            try
            {
                var parsed = UnityEngine.JsonUtility.FromJson<ChatResponseJson>(cleanedJson);
                return new GPTResponseData(parsed.ReplyMessage, parsed.Emotion);
            }
            catch (System.Exception e)
            {
                // JSON 파싱 실패 시, 원문 메시지를 응답으로 반환하고 Emotion은 null로 처리
                UnityEngine.Debug.LogWarning($"JSON parsing failed. Returning raw message. Error: {e.Message}");
                return new GPTResponseData(cleanedJson, null);
            }
        }
        else
        {
            return new GPTResponseData("GPT returned no choices or an empty message.", null);
        }
    }

    // 내부 유틸리티 메서드: 엔티티의 MessageRole을 OpenAI 라이브러리의 Role 타입으로 변환
    private Role ConvertRole(EMessageRole role)
    {
        switch (role)
        {
            case EMessageRole.System:
                return Role.System;
            case EMessageRole.User:
                return Role.User;
            case EMessageRole.Assistant:
                return Role.Assistant;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(role), "Unknown MessageRole");
        }
    }
}

[System.Serializable]
public class ChatResponseJson
{
    public string ReplyMessage;
    public string Emotion;
}