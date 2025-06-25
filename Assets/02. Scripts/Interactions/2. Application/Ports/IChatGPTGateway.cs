using System.Collections.Generic;
using System.Threading.Tasks;

public interface IChatGPTGateway
{
    Task<GPTResponseData> GetChatCompletionAsync(List<ChatMessage> conversationHistory, CharacterData characterData, CharacterData ruleData);
}
