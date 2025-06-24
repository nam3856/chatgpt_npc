using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterExpression", menuName = "Character/Expression")]
public class CharacterExpressionSO : ScriptableObject
{
    public string characterName;
    public List<ExpressionEntry> expressions;

    [System.Serializable]
    public class ExpressionEntry
    {
        public string emotion;
        public List<string> spriteNames;
    }

}