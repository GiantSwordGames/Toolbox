using System;
using System.Collections.Generic;
using System.IO;
using JamKit;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JamKit.AssetRules
{
    public static class AssetRuleMenuItems
    {
        public const string ASSET_MENU_PATH = MenuPaths.CREATE_ASSET_MENU + "/Rules/Asset Rules";
        public const string ASSETS_MENU_PATH = "Assets";

        [MenuItem("Assets/Check Naming Conventions", false, 112)]
        private static void SearchForCrimes()
        {
            List<RuleResult> results = new List<RuleResult>();
            List<WarningRule.WarningResult> warnings = new List<WarningRule.WarningResult>();

            foreach (Object folder in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(folder);
                AssetRuleUtility.SearchForCrimesInFolder(assetPath, results, warnings);
            }
        }

        [MenuItem( MenuPaths.QUICK_CREATE + "Create Asset Naming Rule For This Asset", false, MenuPaths.QUICK_CREATE_PRIORITY)]
        public static void CreateNamingRule()
        {
            RuleBase findAsset = RuntimeEditorHelper.FindAsset<RuleBase>();
            string path = AssetDatabase.GetAssetPath(findAsset);
            string folderPath = Path.GetDirectoryName(path);
            Debug.Log(path);
            Debug.Log(folderPath);
            
            RuleForSystemType rule = ScriptableObject.CreateInstance<RuleForSystemType>();
            rule.example = Selection.activeObject;
            rule.customPrefix = rule.example.GetType().Name +"_";

            string newPath = folderPath + "/Rule_" + rule.example.GetType().Name + ".asset";
            AssetDatabase.CreateAsset(rule, newPath);

            RuleForUnityType loadAssetAtPath = AssetDatabase.LoadAssetAtPath<RuleForUnityType>(newPath);
            RuntimeEditorHelper.SelectAndFocus(loadAssetAtPath);
            Debug.Log(newPath,loadAssetAtPath);
            
            AssetRuleList list = RuntimeEditorHelper.FindAsset<AssetRuleList>();
            list.rules.Add(loadAssetAtPath);
        }

    }
}