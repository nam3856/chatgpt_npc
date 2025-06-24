using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;

public class ScriptableObjectExpressionDataAdapter : IExpressionDataGateway
{
    private Dictionary<string, CharacterExpressionSO> _expressionSOCache;
    private bool _isInitialized = false;
    private const string ALL_EXPRESSIONS_LABEL = "CharacterExpressions";

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
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
            Debug.LogError("[ScriptableObjectExpressionDataAdapter] Adapter not initialized. Call InitializeAsync first.");
            return null;
        }

        if (_expressionSOCache.TryGetValue(characterName, out var expressionSO))
        {
            List<ExpressionEntryData> expressionEntries = new List<ExpressionEntryData>();
            foreach (var entry in expressionSO.expressions)
            {
                expressionEntries.Add(new ExpressionEntryData(entry.emotion, entry.spriteNames));
            }
            return new ExpressionData(expressionSO.characterName, expressionEntries);
        }
        Debug.LogWarning($"[ScriptableObjectExpressionDataAdapter] CharacterExpressionSO not found in cache for name: {characterName}");
        return null;
    }
}