using RichardPieterse.AssetRules;
using UnityEngine;
using Object = System.Object;

namespace RichardPieterse.AssetRules
{
    public abstract class AssetRulePostProcess: ScriptableObject
    {
        public abstract void Process(RuleResult ruleResult);
    }
}