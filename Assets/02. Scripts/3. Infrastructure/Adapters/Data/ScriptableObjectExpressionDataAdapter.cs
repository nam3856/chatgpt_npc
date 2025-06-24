// ScriptableObjectExpressionDataAdapter.cs
// (위치: Infrastructure/Adapters/Data/)

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // For async operations
using UnityEngine.AddressableAssets; // For Addressables
using UnityEngine; // For Debug.LogWarning

// IExpressionDataGateway는 UseCase 계층에서 정의된 인터페이스입니다.
// 이 어댑터는 해당 인터페이스를 구현합니다.

public class ScriptableObjectExpressionDataAdapter : IExpressionDataGateway
{
    // 모든 CharacterExpressionSO를 캐싱하여 사용하는 것이 효율적입니다.
    private Dictionary<string, CharacterExpressionSO> _expressionSOCache;
    private bool _isInitialized = false;

    // Addressable Group 이름 또는 레이블을 사용할 수 있습니다.
    // 여기서는 "CharacterExpressions" 라는 공통 레이블을 사용하여 모든 Expression SO를 로드한다고 가정합니다.
    private const string ALL_EXPRESSIONS_LABEL = "CharacterExpressions";

    // Initialize 메서드를 통해 비동기 초기화를 수행하도록 변경
    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            // Addressables를 사용하여 모든 CharacterExpressionSO 로드
            var handle = Addressables.LoadAssetsAsync<CharacterExpressionSO>(ALL_EXPRESSIONS_LABEL, null);
            await handle.Task;

            _expressionSOCache = handle.Result?.ToDictionary(so => so.characterName, so => so)
                                 ?? new Dictionary<string, CharacterExpressionSO>();
            _isInitialized = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ScriptableObjectExpressionDataAdapter] Failed to load all expression SOs via Addressables: {e.Message}");
            _expressionSOCache = new Dictionary<string, CharacterExpressionSO>();
        }
    }

    public ExpressionData GetExpressionData(string characterName)
    {
        if (!_isInitialized)
        {
            // 초기화되지 않은 경우 경고 또는 예외 처리
            Debug.LogError("[ScriptableObjectExpressionDataAdapter] Adapter not initialized. Call InitializeAsync first.");
            return null;
        }

        if (_expressionSOCache.TryGetValue(characterName, out var expressionSO))
        {
            List<ExpressionEntryData> expressionEntries = new List<ExpressionEntryData>();
            foreach (var entry in expressionSO.expressions)
            {
                // Sprite[] 대신 Sprite 이름을 List<string>으로 변환
                List<string> spriteNames = entry.sprites?.Select(s => s?.name).ToList();
                expressionEntries.Add(new ExpressionEntryData(entry.emotion, spriteNames));
            }
            return new ExpressionData(expressionSO.characterName, expressionEntries);
        }
        Debug.LogWarning($"[ScriptableObjectExpressionDataAdapter] CharacterExpressionSO not found in cache for name: {characterName}");
        return null;
    }
}