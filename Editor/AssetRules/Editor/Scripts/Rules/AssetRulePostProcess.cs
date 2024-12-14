using GiantSword.AssetRules;
using UnityEngine;
using Object = System.Object;

namespace GiantSword.AssetRules
{
    public abstract class AssetRulePostProcess: ScriptableObject
    {
        public abstract void Process(RuleResult ruleResult);
    }
}