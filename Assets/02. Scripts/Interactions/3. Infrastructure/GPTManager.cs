using UnityEngine;
using TMPro;
using System.Collections.Generic;
using OpenAI.Chat;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public class GPTManager : MonoBehaviour, IChatOutputPort
{
    [Header("OpenAI Settings")]
    public OpenAISettings settingsSO;

    [Header("UI References")]
    public TMP_InputField inputField;
    public GameObject LoadingObject;
    [SerializeField] private GameObject ChatPrefab;
    [SerializeField] private GameObject ChatParent;
    [SerializeField] private ScrollRect chatScrollRect;
    public Sprite defaultSprite;

    [Header("Character Image")]
    [SerializeField] private Image characterImageUI;

    private SendChatUseCase _sendChatUseCase;
    private ScriptableObjectCharacterDataAdapter _characterDataGateway;
    private ScriptableObjectExpressionDataAdapter _expressionDataGateway;
    private OpenAIChatGPTAdapter _chatGPTGateway;
    private CharacterImageUIPresenter _characterImagePresenter;

    private List<ChatMessage> _conversationHistory = new List<ChatMessage>();

    public string initialCharacterName; // 초기 로드할 캐릭터 이름
    public string initialRuleName;      // 초기 로드할 규칙 이름

    private EvaluateLikabilityUseCase _evaluateLikabilityUseCase;


    async void Awake()
    {
        _characterDataGateway = new ScriptableObjectCharacterDataAdapter();
        _expressionDataGateway = new ScriptableObjectExpressionDataAdapter();
        _chatGPTGateway = new OpenAIChatGPTAdapter(settingsSO.apiKey);

        await _expressionDataGateway.InitializeAsync();

        _characterImagePresenter = characterImageUI.GetComponent<CharacterImageUIPresenter>();
        if (_characterImagePresenter == null)
        {
            _characterImagePresenter = characterImageUI.gameObject.AddComponent<CharacterImageUIPresenter>();
        }
        _characterImagePresenter.Initialize(_expressionDataGateway, characterImageUI, defaultSprite);


        CharacterData characterData = await _characterDataGateway.GetCharacterData(initialCharacterName);
        CharacterData ruleData = await _characterDataGateway.GetRuleData(initialRuleName);
        CharacterData likabilityRuleData = await _characterDataGateway.GetRuleData("LikabilityRule");

        if (characterData == null)
        {
            DisplayErrorMessage($"Initial character data '{initialCharacterName}' not found.");
            return;
        }
        if (ruleData == null)
        {
            DisplayErrorMessage($"Initial rule data '{initialRuleName}' not found.");
            return;
        }
        if (likabilityRuleData == null)
        {
            DisplayErrorMessage("Likability rule data not found.");
            return;
        }

        var likabilityEvaluator = new OpenAIGPTLikabilityAdapter(settingsSO.apiKey, likabilityRuleData);
        _evaluateLikabilityUseCase = new EvaluateLikabilityUseCase(likabilityEvaluator);

        _sendChatUseCase = new SendChatUseCase(
            _chatGPTGateway,
            this,
            characterData,
            ruleData,
            _conversationHistory,
            _characterImagePresenter
        );

        Debug.Log("GPTManager initialized successfully.");
    }

    public async void OnSubmit()
    {
        string userMessage = inputField.text;
        inputField.text = "";


        await _sendChatUseCase.Execute(userMessage);

        var characterData = _sendChatUseCase.GetCharacterData();
        var request = new LikabilityEvaluationRequest(
        characterName: characterData.Name,
        personalitySummary: characterData.PersonalitySummary,
        affectionScore: characterData.LikabilityScore,
        interactionCount: characterData.InteractionCount,
        playerMessage: userMessage
    );

        try
        {
            int delta = await _evaluateLikabilityUseCase.ExecuteAsync(request);

            characterData.UpdateLikabilityScore(delta);

            Debug.Log($"[호감도] +{delta} → 현재 호감도: {characterData.LikabilityScore}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[호감도 평가 실패] {e.Message}");
        }
    }

    public void DisplayUserMessage(string message)
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

    public void DisplayAiMessage(string characterDisplayName, string replyMessage, string emotion)
    {
        var chatInstance = Instantiate(ChatPrefab, ChatParent.transform).GetComponent<TextHeightSetter>();
        if (chatInstance != null)
        {
            chatInstance.Text = $"{characterDisplayName}: {replyMessage}";
        }
        else
        {
            Debug.LogError("ChatPrefab does not have TextHeightSetter component or it's null.");
        }
        ScrollToBottom();
    }

    public void DisplayErrorMessage(string errorMessage)
    {
        var chatInstance = Instantiate(ChatPrefab, ChatParent.transform).GetComponent<TextHeightSetter>();
        if (chatInstance != null)
        {
            chatInstance.Text = $"오류: {errorMessage}";
            Debug.LogError(errorMessage);
            chatInstance.GetComponent<TMP_Text>().color = Color.red;
        }
        else
        {
            Debug.LogError("ChatPrefab does not have TextHeightSetter component or it's null.");
        }
        ScrollToBottom();
    }

    public void ShowLoadingIndicator()
    {
        LoadingObject.SetActive(true);
    }

    public void HideLoadingIndicator()
    {
        LoadingObject.SetActive(false);
    }

    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }

}
