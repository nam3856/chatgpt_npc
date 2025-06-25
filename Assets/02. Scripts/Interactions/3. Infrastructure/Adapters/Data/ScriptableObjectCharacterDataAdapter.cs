using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System;

public interface ICharacterDataGateway
{
    Task<CharacterData> GetCharacterData(string characterName);
    Task<CharacterData> GetRuleData(string ruleName);
}

public class ScriptableObjectCharacterDataAdapter : ICharacterDataGateway
{
    private const string CHARACTER_LABEL_PREFIX = "Character_";
    private const string RULE_LABEL_PREFIX = "Rule_";

    public async Task<CharacterData> GetCharacterData(string characterName)
    {
        string label = CHARACTER_LABEL_PREFIX + characterName;
        try
        {
            var handle = Addressables.LoadAssetAsync<BaseCharacterSO>(label);
            await handle.Task;

            BaseCharacterSO so = handle.Result;
            if (so != null)
            {
                EPosition parsedPosition = Enum.TryParse(so.position, out EPosition pos) ? pos : EPosition.ManagerCandidate;

                var stats = new HeroineGameStats(
                    so.batting, so.pitching, so.defense, so.speed, so.stamina, so.gameSense,
                    so.leadership, so.strategy, so.motivation, so.focus, so.catchSense
                );

                return new CharacterData(
                    so.characterName,
                    so.characterDisplayName,
                    parsedPosition,
                    so.gptPrompt,
                    so.personalitySummary,
                    stats
                );
            }
            Debug.LogWarning($"[ScriptableObjectCharacterDataAdapter] CharacterSO not found for label: {label}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ScriptableObjectCharacterDataAdapter] Failed to load character '{characterName}' via Addressables: {e.Message}");
            return null;
        }
    }

    public async Task<CharacterData> GetRuleData(string ruleName)
    {
        string label = RULE_LABEL_PREFIX + ruleName;
        try
        {
            var handle = Addressables.LoadAssetAsync<BaseCharacterSO>(label);
            await handle.Task;

            BaseCharacterSO so = handle.Result;
            if (so != null)
            {
                EPosition defaultPosition = EPosition.ManagerCandidate;

                var emptyStats = new HeroineGameStats(
                    0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0
                );

                return new CharacterData(
                    so.characterName,
                    so.characterDisplayName,
                    defaultPosition,
                    so.gptPrompt,
                    so.personalitySummary,
                    emptyStats
                );
            }

            Debug.LogWarning($"[ScriptableObjectCharacterDataAdapter] RuleSO not found for label: {label}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ScriptableObjectCharacterDataAdapter] Failed to load rule '{ruleName}' via Addressables: {e.Message}");
            return null;
        }
    }
}