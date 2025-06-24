// CharacterImageUIPresenter.cs
// (위치: Infrastructure/Adapters/Presenters/)

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets; // For Addressables
using System.Threading.Tasks; // For async operations

// 캐릭터 이미지 업데이트를 위한 인터페이스
public interface ICharacterImageOutputPort
{
    // emotion 대신 최종적으로 로드할 Sprite 이름을 받아오도록 변경하면 더 좋지만,
    // 현재 유스케이스에서 emotion을 반환하므로, Presenter가 emotion을 받아
    // Addressables를 통해 Sprite를 로드하는 방식으로 유지합니다.
    // Task를 반환하도록 변경 (비동기 로드 때문)
    Task UpdateCharacterImage(string characterName, string emotion);
}

public class CharacterImageUIPresenter : MonoBehaviour, ICharacterImageOutputPort
{
    [Header("UI References")]
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite defaultSprite; // 기본 스프라이트 (표정이 없을 때)

    private IExpressionDataGateway _expressionDataGateway; // 데이터를 얻기 위해 Gateway를 주입받습니다.

    // 이미지 스프라이트 Addressable 레이블 접두사
    private const string CHARACTER_EXPRESSION_SPRITE_LABEL_PREFIX = "Expression_"; // 예: "Expression_Player_happy_01"

    public void Initialize(IExpressionDataGateway expressionDataGateway)
    {
        _expressionDataGateway = expressionDataGateway;
    }

    public async Task UpdateCharacterImage(string characterName, string emotion)
    {
        if (characterImage == null)
        {
            Debug.LogError("Character Image is not assigned in CharacterImageUIPresenter.");
            return;
        }
        if (_expressionDataGateway == null)
        {
            Debug.LogError("ExpressionDataGateway is not initialized in CharacterImageUIPresenter.");
            characterImage.sprite = defaultSprite;
            return;
        }

        Sprite targetSprite = defaultSprite; // 기본값은 defaultSprite
        string selectedSpriteName = null;

        ExpressionData expressionData = _expressionDataGateway.GetExpressionData(characterName);

        if (expressionData != null)
        {
            // 1. 요청된 감정 스프라이트 찾기
            var emotionEntry = expressionData.Expressions.Find(e =>
                e.Emotion.Equals(emotion, System.StringComparison.OrdinalIgnoreCase));

            if (emotionEntry != null && emotionEntry.SpriteNames != null && emotionEntry.SpriteNames.Count > 0)
            {
                selectedSpriteName = emotionEntry.SpriteNames[Random.Range(0, emotionEntry.SpriteNames.Count)];
            }
            else
            {
                // 2. 요청된 감정이 없으면 'idle' 감정 스프라이트 찾기
                var idleEntry = expressionData.Expressions.Find(e =>
                    e.Emotion.Equals("idle", System.StringComparison.OrdinalIgnoreCase));

                if (idleEntry != null && idleEntry.SpriteNames != null && idleEntry.SpriteNames.Count > 0)
                {
                    selectedSpriteName = idleEntry.SpriteNames[Random.Range(0, idleEntry.SpriteNames.Count)];
                }
            }

            if (!string.IsNullOrEmpty(selectedSpriteName))
            {
                string addressableKey = $"{characterName}/{selectedSpriteName}"; // Addressable 키 구성 (예: "Player/Player_happy_01")
                                                                                 // 또는 더 복잡한 레이블 체계 (예: "Char_Player_Expr_happy_01")

                try
                {
                    // Addressables를 사용하여 스프라이트 에셋 비동기 로드
                    var handle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
                    await handle.Task;
                    targetSprite = handle.Result;

                    if (targetSprite == null)
                    {
                        Debug.LogWarning($"[CharacterImageUIPresenter] Addressable Sprite '{addressableKey}' not found. Using default sprite.");
                        targetSprite = defaultSprite;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[CharacterImageUIPresenter] Failed to load sprite '{addressableKey}' via Addressables: {e.Message}");
                    targetSprite = defaultSprite;
                }
            }
            else
            {
                Debug.LogWarning($"[CharacterImageUIPresenter] No valid sprite name found for '{emotion}' or 'idle' for character '{characterName}'. Using default sprite.");
            }
        }
        else
        {
            Debug.LogWarning($"[CharacterImageUIPresenter] No expression data found for character '{characterName}'. Using default sprite.");
        }

        characterImage.sprite = targetSprite;
    }
}