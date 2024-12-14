using UnityEditor;
using UnityEngine;

namespace GiantSword.AssetRules
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RuleForUnityType))]
    public class NamingRuleForUnityTypeEditor : AssetNamingRuleEditor<RuleForUnityType>
    {
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RuleForTexture))]
    public class NamingRuleForTextureEditor : AssetNamingRuleEditor<RuleForTexture>
    {
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RuleForSystemType))]
    public class AssetNamingRuleEditor : AssetNamingRuleEditor<RuleForSystemType>
    {
    }    
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RuleForFolder))]
    public class RuleForFolderEditor : AssetNamingRuleEditor<RuleForFolder>
    {
    }

    public class AssetNamingRuleEditor<T> : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RuleBase ruleBase = target as RuleBase;
            GUILayout.Space(30);

            if (ruleBase.example == null)
            {
                GUILayout.Label("No Example Type Assigned");
            }
            else
            {
                GUIStyle heading = new GUIStyle(GUI.skin.label);
                heading.fontStyle = FontStyle.Bold;
                GUILayout.Label($"Preview Output:", heading);

                GUILayout.Label($"Type: {ruleBase.type}");
                string example = ruleBase.GetExample();
                GUILayout.Label($"Example: {example}");

                // string withCategory = ruleBase.ApplyCategory(example);
                // if (example != withCategory)
                    // GUILayout.Label("Result: " + withCategory);

                if (ruleBase._test)
                {
                    string assetPath = AssetDatabase.GetAssetPath(ruleBase._test);
                    GUILayout.Label("");
                    GUILayout.Label($"Asset Test Output:", heading);
                    string test = ruleBase.Format( assetPath);
                    GUILayout.Label($"Test:   {assetPath}");
                    GUILayout.Label($"Test Type:   {ruleBase._test.GetType()}");

                    bool doesRuleApply = ruleBase.DoesRuleApply(ruleBase._test, assetPath);
                    GUILayout.Label($"Rule Applies:   {doesRuleApply}");
                    GUILayout.Label($"Result: {test}");
                }

                if (ruleBase._pathTest != "")
                {
                    GUILayout.Label($"");
                    GUILayout.Label($"Path Test Output:", heading);
                    RuleResult ruleResult = ruleBase.GetResult(null,ruleBase._pathTest);
                    GUILayout.Label(ruleBase._pathTest);
                    GUILayout.Label(ruleResult.newPath);
                    GUILayout.Label(ruleResult.customMessage);
                }
            }

            GUILayout.Label("");

            if (GUILayout.Button("Search for Crimes"))
            {
                AssetRuleUtility.SearchForCrimesByRule(ruleBase);
            }
        }
    }
}