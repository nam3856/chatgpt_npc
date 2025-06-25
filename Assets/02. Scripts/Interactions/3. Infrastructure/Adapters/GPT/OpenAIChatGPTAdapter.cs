using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

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
            cleanedJson = WrapAsJson(cleanedJson); // 여기 추가

            try
            {
                var parsed = JsonUtility.FromJson<ChatResponseJson>(cleanedJson);
                return new GPTResponseData(parsed.ReplyMessage, parsed.Emotion);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"JSON parsing failed. Returning raw message. Error: {e.Message}, {cleanedJson}");
                string safeMessage = ExtractPlaintextMessage(cleanedJson);
                return new GPTResponseData(safeMessage, InferEmotionLocally(cleanedJson));
            }
        }
        else
        {
            return new GPTResponseData("GPT returned no choices or an empty message.", null);
        }
    }
    private string WrapAsJson(string content)
    {
        // "ReplyMessage": "...", "Emotion": "..." 이런 형식일 때 감싸기
        if (content.TrimStart().StartsWith("\"ReplyMessage\""))
        {
            return "{" + content.Trim().Trim(',') + "}";
        }
        return content;
    }
    // 엔티티의 MessageRole을 OpenAI 라이브러리의 Role 타입으로 변환
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
    private string ExtractPlaintextMessage(string raw)
    {
        var match = Regex.Match(raw, @"""ReplyMessage""\s*:\s*""([^""]+)""");
        if (match.Success)
            return match.Groups[1].Value;

        return raw.Trim(); // 그냥 전체 메시지를 fallback으로
    }

    private string InferEmotionLocally(string message)
    {
        if (message.Contains("좋아") || message.Contains("행복") || message.Contains("고마")) return "Happy";
        if (message.Contains("화나") || message.Contains("짜증") || message.Contains("그만")) return "Angry";
        if (message.Contains("긴장") || message.Contains("불안")) return "Nervous";
        if (message.Contains("피곤") || message.Contains("졸려")) return "Tired";
        if (message.Contains("신나") || message.Contains("재밌")) return "Excited";
        if (message.Contains("저기...") || message.Contains("부끄")) return "Embarrassed";
        if (message.Contains("슬퍼") || message.Contains("우울")) return "Sad";
        if (message.Contains("놀랐") || message.Contains("헉")) return "Surprised";
        return "Neutral";
    }
}

[System.Serializable]
public class ChatResponseJson
{
    public string ReplyMessage;
    public string Emotion;
}