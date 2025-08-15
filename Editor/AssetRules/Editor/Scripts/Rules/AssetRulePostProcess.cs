using JamKit.AssetRules;
using UnityEngine;
using Object = System.Object;

namespace JamKit.AssetRules
{
    public abstract class AssetRulePostProcess: ScriptableObject
    {
        public abstract void Process(RuleResult ruleResult);
    }
}