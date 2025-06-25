using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public interface ICharacterImageOutputPort
{
    Task UpdateCharacterImage(string characterName, string emotion);
}

public class CharacterImageUIPresenter : MonoBehaviour, ICharacterImageOutputPort
{
    private Image _characterImage;
    private Sprite _defaultSprite;

    private IExpressionDataGateway _expressionDataGateway;

    public void Initialize(IExpressionDataGateway expressionDataGateway, Image characterImageRef, Sprite defaultSpriteRef)
    {
        _expressionDataGateway = expressionDataGateway;
        _characterImage = characterImageRef;
        _defaultSprite = defaultSpriteRef;

        if (_characterImage == null)
        {
            Debug.LogError("[CharacterImageUIPresenter] Character Image reference is null during initialization.");
        }
        if (_defaultSprite == null)
        {
            Debug.LogWarning("[CharacterImageUIPresenter] Default Sprite reference is null. Character image may not display fallback.");
        }
    }

    public async Task UpdateCharacterImage(string characterName, string emotion)
    {
        if (_characterImage == null)
        {
            Debug.LogError("Character Image is not assigned in CharacterImageUIPresenter.");
            return;
        }
        if (_expressionDataGateway == null)
        {
            Debug.LogError("ExpressionDataGateway is not initialized in CharacterImageUIPresenter.");
            _characterImage.sprite = _defaultSprite;
            return;
        }

        Sprite targetSprite = _defaultSprite;
        string selectedSpriteName = null;

        ExpressionData expressionData = _expressionDataGateway.GetExpressionData(characterName);

        if (expressionData != null)
        {
            var emotionEntry = expressionData.Expressions.Find(e =>
                e.Emotion.Equals(emotion, System.StringComparison.OrdinalIgnoreCase));

            if (emotionEntry != null && emotionEntry.SpriteNames != null && emotionEntry.SpriteNames.Count > 0)
            {
                selectedSpriteName = emotionEntry.SpriteNames[Random.Range(0, emotionEntry.SpriteNames.Count)];
            }
            else
            {
                var idleEntry = expressionData.Expressions.Find(e =>
                    e.Emotion.Equals("idle", System.StringComparison.OrdinalIgnoreCase));

                if (idleEntry != null && idleEntry.SpriteNames != null && idleEntry.SpriteNames.Count > 0)
                {
                    selectedSpriteName = idleEntry.SpriteNames[Random.Range(0, idleEntry.SpriteNames.Count)];
                }
            }
            Debug.Log(emotion);
            if (!string.IsNullOrEmpty(selectedSpriteName))
            {
                string addressableKey = $"{characterName}/{selectedSpriteName}";
                try
                {
                    var handle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
                    await handle.Task;
                    targetSprite = handle.Result;
                    if (targetSprite == null)
                    {
                        Debug.LogWarning($"[CharacterImageUIPresenter] Addressable Sprite '{addressableKey}' not found. Using default sprite.");
                        targetSprite = _defaultSprite;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[CharacterImageUIPresenter] Failed to load sprite '{addressableKey}' via Addressables: {e.Message}");
                    targetSprite = _defaultSprite;
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

        _characterImage.sprite = targetSprite;
    }
}