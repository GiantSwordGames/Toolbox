using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GiantSword.AssetRules
{
     class RulesAssetPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            
            if (AssetRulePreferences.checkRulesOnAssetImport == false)
                return;

            List<RuleResult> results = new List<RuleResult>();
            List<WarningRule.WarningResult> warnings = new List<WarningRule.WarningResult>();

            for (var i = 0; i < importedAssets.Length; i++)
            {
                string str = importedAssets[i];
                AssetRuleUtility.CheckRule(str, results, warnings);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                AssetRuleUtility.CheckRule(movedAssets[i], results, warnings);
            }

            if (results.Count > 0)
            {
                if (AssetRulePreferences.autoApplyNamingConventions)
                {
                    foreach (RuleResult ruleResult in results)
                    {
                        ruleResult.ApplyFix();
                        Debug.Log("Fixed " + ruleResult.newPath, ruleResult.asset);
                    }
                }
                else
                {
                    AssetRuleUtility.DisplayPopup(results, warnings);
                }
            }
        }
    }
}