using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantSword.AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.ASSET_MENU_PATH )]
    public class RuleForTexture: RuleBase
    {
        public TextureImporterType textureType;
        public string contains;
        public override bool DoesRuleApply(Object asset, string assetPath)
        {
            if (asset is Texture2D)
            {
                string path = assetPath;

                try
                {
                    TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

                
                    if (importer.textureType != textureType)
                        return false;


                    if (string.IsNullOrWhiteSpace(contains) == false)
                    {
                        if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path), $"{contains}", RegexOptions.IgnoreCase) == false)
                        {
                            return false;
                        }
                    }
                    return true;


                }
                catch (Exception e)
                {
                    Debug.Log(path);
                  Debug.LogException(e);
                }

            

            }

            return false;
        }
    }
}