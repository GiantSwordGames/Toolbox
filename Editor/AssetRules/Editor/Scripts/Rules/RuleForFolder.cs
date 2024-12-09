using System.IO;
using UnityEditor;
using UnityEngine;

namespace RichardPieterse.AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.ASSET_MENU_PATH +"/Rule For Folder" )]
    public class RuleForFolder : RuleBase
    {
        public override bool DoesRuleApply(Object asset, string assetPath)
        {
            return Directory.Exists(assetPath) && base.DoesRuleApply(asset, assetPath);
        }

        public override RuleResult GetResult(Object asset, string assetPath)
        {
            // RuleResult ruleResult = base.GetResult(asset, assetPath);
             assetPath =assetPath.Replace("\\", "/") ;
             // assetPath.Substring(0, assetPath.Length - 1);//re

             
             var lastFolderName = new DirectoryInfo(assetPath).Name;
            var validatedFolderName = AssetRuleUtility.ToTitleCase(lastFolderName);
            validatedFolderName = RegexReplace(lastFolderName, _regexReplaceInDirectories);
            string newPath = assetPath.Replace(lastFolderName, validatedFolderName);

            RuleResult result = new RuleResult()
            {
                asset = asset,
                oldPath = assetPath,
                newPath = newPath,
                rule = this,
                triggerReevaluation = true,
                commonPath = AssetRuleUtility.GetCommon(assetPath, newPath)
            };
            
            if (assetPath != newPath)
            {
                result.requiresFix = true;
                result.fixFunc = () =>
                {
                    AssetDatabase.Refresh();
                    return string.IsNullOrEmpty(AssetDatabase.RenameAsset(result.oldPath, validatedFolderName));
                };
            }
            // bool isEmpty = RemoveEmptyFolders.IsEmpty(assetPath);
            // if (isEmpty)
            // {
            //     ruleResult.requiresFix = true;
            //     ruleResult.customMessage = "Remove Empty Folder";
            //     ruleResult.fixAction = () =>                         AssetDatabase.DeleteAsset(ruleResult.oldPath);
            // }
            
            return result;
        }
    }
}