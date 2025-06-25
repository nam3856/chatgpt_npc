using OpenAI.Chat;
using OpenAI.Models;
using OpenAI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class OpenAIGPTLikabilityAdapter : ILikabilityEvaluator
{
    private readonly string _apiKey;
    private readonly CharacterData _ruleData;

    public OpenAIGPTLikabilityAdapter(string apiKey, CharacterData ruleData)
    {
        _apiKey = apiKey;
        _ruleData = ruleData;
    }

    public async Task<int> EvaluateAsync(LikabilityEvaluationRequest request)
    {
        var systemPrompt = $@"
{_ruleData.GptPrompt}

Context:
- Character: {request.CharacterName}
- Personality: {request.PersonalitySummary}
- Current Affection: {request.AffectionScore}
- Interaction Count: {request.InteractionCount}
- Player said: ""{request.PlayerMessage}""
";

        var messages = new List<Message>
        {
            new Message(Role.System, systemPrompt)
        };

        var api = new OpenAIClient(_apiKey);
        var response = await api.ChatEndpoint.GetCompletionAsync(new ChatRequest(messages, Model.GPT4o));

        var content = response.FirstChoice.Message.Content.ToString().Trim();

        if (int.TryParse(content, out int result))
        {
            return Mathf.Clamp(result, -5, 5);
        }

        Debug.LogWarning($"[LikabilityAdapter] Unexpected response: {content}");
        return 0;
    }
}
