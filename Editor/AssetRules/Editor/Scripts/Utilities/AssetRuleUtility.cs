using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantSword.AssetRules
{
    public static class AssetRuleUtility
    {
        private static EditorAssetList<WarningRule> warningRules = default;
        private static List<AbstractRule> rules = instance.rules;

        private static AssetRuleList _instance;
        public static AssetRuleList instance
        {
            get
            {
                if (_instance == null)
                {
                    string guid = AssetDatabase.FindAssets($"t: {typeof(AssetRuleList)} AssetRulePriorityList")[0];
                    _instance = AssetDatabase.LoadAssetAtPath<AssetRuleList>(AssetDatabase.GUIDToAssetPath(guid));
                }

                return _instance;
            }
        }

        internal static void SearchForCrimesInFolder(string folder, List<RuleResult> ruleResults,
            List<WarningRule.WarningResult> warningResults)
        {
            List<string> fileEntries = new List<string>();

            if (Directory.Exists(folder))
            {
                fileEntries.AddRange(Directory.GetFiles($"{Application.dataPath}/{folder.Replace("Assets/", "")}/", "*",
                    SearchOption.AllDirectories));
                fileEntries.AddRange(Directory.GetDirectories(
                    $"{Application.dataPath}/{folder.Replace("Assets/", "")}/", "*",
                    SearchOption.AllDirectories));
            }

            fileEntries.Add(folder);

            List<string> filteredPaths = new List<string>();

            foreach (string entry in fileEntries)
            {
                if (entry.Contains(".meta"))
                {
                    continue;
                }

                string path = entry.Replace(Application.dataPath, "Assets");
                filteredPaths.Add(path);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

                if (asset)
                {
                    foreach (WarningRule warningRule in warningRules)
                    {
                        CheckWarning(warningResults, warningRule, asset);
                    }

                    foreach (RuleBase rule in rules)
                    {
                        try
                        {
                            if (rule.DoesRuleApply(asset, path))
                            {
                                CheckRule(path, rule, ruleResults);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"{rule} {asset} {asset}", asset);
                            Debug.LogException(e);
                        }
                    }
                }
            }

            DisplayPopup(ruleResults, warningResults);
        }

        private static void CheckWarning(List<WarningRule.WarningResult> warningResults, WarningRule warningRule,
            Object asset)
        {
            if (warningResults != null)
            {
                WarningRule.WarningResult warning = warningRule.CheckForWarning(asset);
                if (warning != null)
                {
                    warningResults.Add(warning);
                }
            }
        }

        public static void DisplayPopup(List<RuleResult> ruleResults,
            List<WarningRule.WarningResult> warningResults)
        {
            AssetRuleWindow.Get(ruleResults, warningResults);
        }

        public static void SearchForCrimesByRule(RuleBase ruleBase, string rootFolder = "")
        {
            List<RuleResult> ruleResults = new List<RuleResult>();
            List<WarningRule.WarningResult> warningResults = new List<WarningRule.WarningResult>();

            CheckRule(ruleBase, ruleResults, warningResults);

            AssetRuleWindow.Get(ruleResults, warningResults);
        }

        private static void CheckRule(RuleBase ruleBase, List<RuleResult> ruleResults,
            List<WarningRule.WarningResult> warningResults)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{ruleBase.typeName}");
            for (var i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                CheckRule(assetPath, ruleBase, ruleResults);
            }
        }

        public static void CheckRule(string path, List<RuleResult> ruleResults,
            List<WarningRule.WarningResult> warningResults)
        {
            if (CheckForIgnoredDirectories(path)) return;

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            // Debug.Log($"CheckRule on asset: {path}");

            if (asset)
            {
                foreach (WarningRule warningRule in warningRules)
                {
                    CheckWarning(warningResults, warningRule, asset);
                }

                foreach (RuleBase rule in rules)
                {
                    if (rule != null)
                    {
                        if (rule.DoesRuleApply(asset, path))
                        {
                            CheckRule(path, rule, ruleResults);
                        }
                    }
                }
            }
        }

        private static bool CheckForIgnoredDirectories(string path)
        {
            
            
            
            if (Regex.IsMatch(path, "^(Packages/)"))
            {
                return true;
            }

            if (Regex.IsMatch(path, "^(Assets/Plugins/)"))
            {
                return true;
            }

            if (path.Contains("AddressableAssetsData") || path.Contains("Assets") == false)
            {
                return true;
            }

            return false;
        }

        private static void CheckRule(string assetPath, RuleBase ruleBase,
            List<RuleResult> ruleResults)
        {
            if (Regex.IsMatch(assetPath, "^(Packages/)"))
            {
                return;
            }

            if (Regex.IsMatch(assetPath, "^(Assets/Plugins/)"))
            {
                return;
            }

            if (Regex.IsMatch(assetPath, "^(Assets/Editor Default Resources/)"))
            {
                return;
            }

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (AssetDatabase.IsMainAsset(asset) == false)
            {
                return;
            }

            if (ruleBase.DoesRuleApply(asset, assetPath) == false)
            {
                return;
            }

            RuleResult result = ruleBase.GetResult(asset, assetPath);

            if (result.requiresFix)
                ruleResults.Add(result);
        }

        public static string GetCommon(string s1, string s2)
        {
            string common = "";
            for (int i = 0; i < s1.Length; i++)
            {
                if (i >= s2.Length)
                {
                    break;
                }

                if (s1[i] == s2[i])
                {
                    common += s1[i];
                }
                else
                {
                    break;
                }
            }

            int newLength = common.Length;
            for (int i = common.Length - 1; i >= 0; i--)
            {
                if (common[i] == '/' || common[i] == '\\')
                {
                    break;
                }

                newLength = i;
            }

            try
            {
                common = common.Substring(0, newLength - 1);
            }
            catch 
            {
                Debug.Log($"{common} {newLength} {s1} {s1}");
            }

            return common;
        }

        public static string GetRelativePath(string absolutePath)
        {
            return absolutePath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
        }

        public static void GetUnique(ref string s1, ref string s2)
        {
            string[] list1 = s1.Split(" ");
            string[] list2 = s2.Split(" ");

            int strip = 0;
            for (int i = 0; i < list1.Length; i++)
            {
                if (i >= list2.Length)
                {
                    break;
                }

                if (list1[i] != list2[i])
                {
                    strip = i;
                }
                else
                {
                    break;
                }
            }

            s1 = "";
            for (int i = strip; i < list1.Length; i++)
            {
                s1 += list1[i] + " ";
            }

            s1 = s1.Trim();

            s2 = "";
            for (int i = strip; i < list2.Length; i++)
            {
                s2 += list2[i] + " ";
            }

            s2 = s2.Trim();
        }

        // Returns true if the asset path was successfully changed.
        public static bool MoveAndRename(string oldPath, string newPath)
        {
            if (oldPath.Equals(newPath))
                return false;
            
            string newDirectoryPath = Path.GetDirectoryName(newPath);
            if (newDirectoryPath == null)
                return false;
            
            Directory.CreateDirectory(newDirectoryPath);
            AssetDatabase.ImportAsset(newDirectoryPath);
            AssetDatabase.Refresh();
            string resultMessage = AssetDatabase.MoveAsset(oldPath, newPath);
            return string.IsNullOrEmpty(resultMessage);
        }

        public static string ToTitleCase(string str)
        {
            var tokens = str.Split(new[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token == token.ToUpper()
                    ? token
                    : token.Substring(0, 1).ToUpper() + token.Substring(1);
            }

            return string.Join("", tokens);
        }
    }
}