using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace GiantSword.AssetRules
{
    public abstract class RuleBase : AbstractRule
    {
        static CultureInfo info = new CultureInfo("en-US", false);

   
     
        [Space]
        
        [Header("Settings:")]
        
        [SerializeField] protected string _customPrefix;
        [SerializeField] protected string _customPostfix;
        [SerializeField] protected string _regexValidation;
        [SerializeField] protected bool _removeSpaces = true;
        [SerializeField] protected bool _capitalize = true;
        [SerializeField] protected bool _replaceSpacesWithUnderscores = true;
        [SerializeField] protected bool _removeHyphens = true;
     
        [Tooltip("Specifically for environment folder assets that need to include there super category in there name")]
        [SerializeField] protected bool _includeGrandparentInName = false;
        [FormerlySerializedAs("_category")] [SerializeField] protected string _parentFolder;
        [SerializeField] protected string _absolutePath;

        [SerializeField] protected RegexRule[] _regexReplaceFromName;
        [SerializeField] protected RegexRule[] _regexReplaceInDirectories;
        [SerializeField] protected string[] _regexRemoveFromPrefix;
        [SerializeField] protected AssetRulePostProcess[] _postProcesses;

        public string customPrefix
        {
            get => _customPrefix;
            set => _customPrefix = value;
        }


        private void OnValidate()
        {
            _parentFolder = _parentFolder.Trim();
        }

        public void ApplyPostProcesses(RuleResult ruleResult)
        {
            foreach (var postProcess in _postProcesses)
            {
                if (postProcess)
                    postProcess.Process(ruleResult);
            }
        }

        public bool IsRegexValid(string assetName)
        {
            return Regex.IsMatch(assetName, _regexValidation);
        }

        public static string RegexReplace(string assetName, RegexRule[] list)
        {
            if (list == null)
                return assetName;

            string name = assetName;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].disable == false)
                    name = Regex.Replace(name, list[i].replace, list[i].with);
            }

            return name;
        }

        public static string RegexRemove(string assetName, string[] list)
        {
            if (list == null)
                return assetName;

            string name = assetName;
            for (int i = 0; i < list.Length; i++)
            {
                name = Regex.Replace(name, list[i], "");
            }

            return name;
        }

       
        
        public string GetExample()
        {
            return Format("AssetPath/Example");
        }

        public  string Format(string assetPath)
        {
            if (ShouldIgnore(assetPath))
                return assetPath;
            
            var formattedDirectory = FormatDirectoryPath(assetPath);
            var formattedName = FormatName(assetPath, formattedDirectory);

            return $"{formattedDirectory}{formattedName}{Path.GetExtension(assetPath)}";
        }

        private string FormatDirectoryPath(string assetPath)
        {
            if ( string.IsNullOrEmpty( _absolutePath) == false)
            {
                return _absolutePath;
            }
            
            string formattedDirectory = Path.GetDirectoryName(assetPath).Replace("\\", "/") + "/";

            
            if (_capitalize)
            {
                 formattedDirectory = Regex.Replace(formattedDirectory, @"(^|[^a-zA-Z])([a-z])", match => 
                    match.Groups[1].Value + match.Groups[2].Value.ToUpper()
                );
            }
            
            if (_removeHyphens)
                formattedDirectory = formattedDirectory.Replace("-", "");

            
            if (_removeSpaces)
                formattedDirectory = formattedDirectory.Replace(" ", "");

            if (_replaceSpacesWithUnderscores)
                formattedDirectory = formattedDirectory.Replace(" ", "_");

            formattedDirectory = AssetRuleUtility.ToTitleCase(formattedDirectory);
            
            formattedDirectory = RegexReplace(formattedDirectory, _regexReplaceInDirectories);

            if (!string.IsNullOrEmpty(_parentFolder))
            {
                string parentName;
                DirectoryInfo parent = Directory.GetParent(assetPath);
                parentName = parent.Name;

                if (parentName != "Resources" && Regex.IsMatch(formattedDirectory, $"/{_parentFolder}\\/?$") == false)
                {
                    formattedDirectory = $"{formattedDirectory}{_parentFolder}/";
                }
            }

            return formattedDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="category"></param>
        /// <returns>if returns path to category if found, else returns the submitted path unchanged</returns>
        private static string CheckForMatchingCategoryInGrandparent(string assetPath, string category)
        {
            DirectoryInfo parent = Directory.GetParent(assetPath);
            DirectoryInfo grandParent = parent.Parent;

            string path = grandParent.FullName;
            path += "/" + category;

            if (Directory.Exists(path))
            {
                return AssetRuleUtility.GetRelativePath( path) + "/";
            }

            return assetPath;
        }

        private string FormatName(string assetPath, string formattedDirectoryPath)
        {
            string formattedName = Path.GetFileNameWithoutExtension(assetPath);

            if (_capitalize)
            {
                formattedName = Regex.Replace(formattedName, @"(^|[^a-zA-Z])([a-z])", match => 
                    match.Groups[1].Value + match.Groups[2].Value.ToUpper()
                );
            }
            
            if (_removeHyphens)
                formattedName = formattedName.Replace("-", "");

            if (_removeSpaces)
                formattedName = formattedName.Replace(" ", "");

            if (_replaceSpacesWithUnderscores)
                formattedName = formattedName.Replace(" ", "_");

            formattedName = RegexReplace(formattedName, _regexReplaceFromName);

            if (_includeGrandparentInName)
            {
                string[] folders = formattedDirectoryPath.Split("/");
                for (int i = folders.Length - 1; i >= 0; i--)
                {
                    if (folders[i] != "" && folders[i] != _parentFolder)
                    {
                        formattedName = formattedName.Replace(folders[i]  +"_", "");
                        formattedName = $"{folders[i]}_{formattedName}";
                        break;
                    }
                }
             
            }
            
            foreach (var removeFromPrefix in _regexRemoveFromPrefix)
            {
                formattedName = Regex.Replace(formattedName, $"^{removeFromPrefix}", "");
            }
            
            if (customPrefix != "")
            {
                formattedName = Regex.Replace(formattedName, customPrefix, "");

                if (Regex.IsMatch(formattedName, $"^({customPrefix})") == false) // check if prefix already exists
                {
                    formattedName = $"{customPrefix}{formattedName}";
                }
            }


            if (_customPostfix != "" && Regex.IsMatch(formattedName, $"({_customPostfix}$)") == false)
                formattedName = $"{formattedName}{_customPostfix}";


            formattedName = formattedName.Replace("__", "_");
            formattedName = formattedName.Replace("__", "_");
            if (formattedName.Last() == '_')
            {
                formattedName = formattedName.Substring(0, formattedName.Length - 1);
            }
            
            
            
            return formattedName;
        }

        public virtual RuleResult GetResult(Object asset, string assetPath)
        {
            RuleResult result;

            assetPath = assetPath.Replace("\\", "/");
            string newPath = Format(assetPath);

            // Apply fix action tags for Create Folder, Move Asset to Folder, and Rename Asset
            RuleResult.FixActionTags fixActionTags = RuleResult.FixActionTags.None;
            if (!Directory.Exists(Directory.GetParent(newPath).FullName))
            {
                fixActionTags |= RuleResult.FixActionTags.CreateFolder | RuleResult.FixActionTags.MoveAsset;
            }
            else if (!Directory.GetParent(newPath).FullName.Equals(Directory.GetParent(assetPath).FullName))
            {
                fixActionTags |= RuleResult.FixActionTags.MoveAsset;
            }
            if (!Path.GetFileNameWithoutExtension(newPath).Equals(Path.GetFileNameWithoutExtension(assetPath)))
            {
                fixActionTags |= RuleResult.FixActionTags.RenameAsset;
            }
            
            result = new RuleResult()
            {
                asset =  asset,
                oldPath = assetPath,
                newPath = newPath,
                rule = this,
                commonPath = AssetRuleUtility.GetCommon(assetPath, newPath ), 
                fixActionTags = fixActionTags
            };
            if (assetPath != newPath)
            {
                result.requiresFix = true;
                result.fixFunc = () => AssetRuleUtility.MoveAndRename(result.oldPath, result.newPath);
            }

            return result;
        }

    }

    public class RuleResult
    {
        [Flags]
        public enum FixActionTags
        {
            None = 0,
            CreateFolder = 1 << 0,  // 0001
            MoveAsset = 1 << 1,     // 0010
            RenameAsset = 1 << 2,   // 0100
        }
        
        public Object asset;
        public string oldPath;
        public string newPath;
        public string commonPath;
        public bool selected = true;
        public RuleBase rule;
        public bool requiresFix;
        public string customMessage;
        public Func<bool> fixFunc;
        public bool triggerReevaluation = false;
        public FixActionTags fixActionTags = FixActionTags.None;
        
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is RuleResult result)
            {
                return asset.Equals(result.asset);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (asset == null)
                return 0;
                
            return asset.GetHashCode();
        }

        public bool ApplyFix()
        {
            bool success = fixFunc.Invoke();
            rule.ApplyPostProcesses(this);
            
            return success;
        }
    }
}