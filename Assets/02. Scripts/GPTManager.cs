using UnityEngine;
using TMPro;
using System.Collections.Generic;
using OpenAI.Chat;
using OpenAI.Models;
using OpenAI;
using UnityEngine.UI;
using OpenAI.Audio;
using System.Linq;

public class GPTManager : MonoBehaviour
{
    public OpenAISettings settingsSO; // 인스펙터에 Drag & Drop
    public TMP_InputField inputField;
    public BaseCharacterSO characterSO;
    public BaseCharacterSO RuleSO;
    public GameObject LoadingObject;
    private List<Message> conversationHistory = new List<Message>(); // 대화 기록을 저장할 리스트
    [SerializeField] private GameObject ChatPrefab;
    [SerializeField] private GameObject ChatParent;
    [SerializeField] private ScrollRect chatScrollRect;
    public Sprite defaultSprite;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public Voice voice = Voice.Sage;

    [SerializeField] private Image characterImage;
    [SerializeField] private CharacterExpressionSO[] allExpressions;

    private Dictionary<string, CharacterExpressionSO> expressionLookup;



    void Awake()
    {
        expressionLookup = allExpressions.ToDictionary(e => e.characterName, e => e);
    }


    public void ApplyEmotion(string characterName, string emotion)
    {
        Debug.Log(emotion);
        if (expressionLookup.TryGetValue(characterName, out var expressionSO))
        {
            var sprite = expressionSO.GetRandomSpriteForEmotion(emotion);
            if (sprite != null)
            {
                characterImage.sprite = sprite;
            }
        }
    }
    public void OnSubmit()
    {
        SendMessageToGPTAsync(inputField.text);
        inputField.text = ""; // 메시지 전송 후 입력 필드 비우기
    }
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // 먼저 레이아웃을 강제로 업데이트!
        chatScrollRect.verticalNormalizedPosition = 0f; // 0이면 맨 아래!
    }
    private async void SendMessageToGPTAsync(string userMessage)
    {
        if (characterSO == null)
        {
            Debug.LogError("Character ScriptableObject is not assigned in GPTManager.");
            DisplayChat("Error: Character data missing.");
            return;
        }
        if (string.IsNullOrEmpty(settingsSO.apiKey))
        {
            Debug.LogError("API Key is not set in OpenAISettings.");
            DisplayChat("Error: API Key is missing.");
            return;
        }
        if (string.IsNullOrEmpty(userMessage))
        {
            Debug.LogError("User message is empty.");
            DisplayChat("Error: Please enter a message.");
            return;
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned in GPTManager. Please assign an AudioSource component.");
            DisplayChat("Error: Audio system not set up.");
            return;
        }

        var api = new OpenAIClient(settingsSO.apiKey);

        // 대화가 처음 시작될 때만 시스템 프롬프트 추가
        if (conversationHistory.Count == 0 && !string.IsNullOrWhiteSpace(characterSO.gptPrompt))
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
{RuleSO.gptPrompt}
{characterSO.gptPrompt}
";

            conversationHistory.Add(new Message(Role.System, systemPrompt));
        }

        // 사용자의 현재 메시지를 대화 기록에 추가
        conversationHistory.Add(new Message(Role.User, userMessage));

        // UI에 사용자 메시지 먼저 표시
        DisplayChat("나: " + userMessage);

        LoadingObject.SetActive(true); // 로딩 UI 활성화

        try
        {
            var chatRequest = new ChatRequest(
                conversationHistory,
                Model.GPT4o
            );

            var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            var choice = response.FirstChoice;
            var aiMessage = choice?.Message?.Content as string;
            //var audioClip = choice?.Message?.AudioOutput?.AudioClip;

            if (!string.IsNullOrEmpty(aiMessage))
            {
                string cleanedJson = aiMessage.Replace("```json", "").Replace("```", "").Trim();
                try
                {
                    var parsed = JsonUtility.FromJson<ChatResponseJson>(cleanedJson);
                    DisplayChat($"{characterSO.characterName}: {parsed.ReplyMessage}");

                    // 나중에 감정 애니메이션 처리도 여기에!
                    ApplyEmotion(characterSO.characterName, parsed.Emotion);

                    // TTS용으로는 parsed.ReplyMessage만 사용!
                }
                catch
                {
                    Debug.LogWarning("JSON 파싱 실패! 원문 출력");
                    DisplayChat($"{characterSO.characterName}: {cleanedJson}");
                }
            }
            else
            {
                DisplayChat("GPT 응답 없음.");
                Debug.LogWarning("GPT returned no choices or an empty message.");
            }
        }
        catch (System.Exception e)
        {
            DisplayChat("API 오류: " + e.Message);
            Debug.LogError("OpenAI API Error: " + e.Message);
        }
        finally
        {
            LoadingObject.SetActive(false); // 로딩 UI 비활성화
            ScrollToBottom();
        }
    }

    // 채팅 메시지를 UI에 표시하는 헬퍼 메서드
    private void DisplayChat(string message)
    {
        var chatInstance = Instantiate(ChatPrefab, ChatParent.transform).GetComponent<TextHeightSetter>();
        if (chatInstance != null)
        {
            chatInstance.Text = message;
        }
        else
        {
            Debug.LogError("ChatPrefab does not have TextHeightSetter component or it's null.");
        }
        ScrollToBottom();
    }
}

[System.Serializable]
public class ChatResponseJson
{
    public string ReplyMessage;
    public string Emotion;
}