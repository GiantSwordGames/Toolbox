using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantSword.AssetRules
{
    // [CreateAssetMenu(menuName = AssetRuleMenuItems.ASSET_MENU_PATH + "/Rule for Addressable")]
    public abstract class AbstractRule : ScriptableObject
    {
        [SerializeField] public bool _enabled = true;
        [Space]

        [SerializeField] protected Object _example;
        [Space]
        [Header("Tests")] 

        [SerializeField] public Object _test;
        [SerializeField] public string _pathTest;

        [Header("Filters")] 
        [Tooltip("Ignored Folders and Files. Uses Regex")]
        [SerializeField] public string[] _exceptions;
        
        [Tooltip("Required to be in path for rule to take effect.  Uses Regex")]
        [SerializeField] public string[] _pathRequirements;
        [SerializeField] public int order = 0;

        
        public Type type => example.GetType();
        public string typeName => example.GetType().Name;

        public Object example
        {
            get => _example;
            set => _example = value;
        }

        public bool ShouldIgnore(string assetName)
        {
            if (_enabled == false)
                return true;

            string name = assetName;

            if (_exceptions != null)
            {
                for (int i = 0; i < _exceptions.Length; i++)
                {
                    if (Regex.IsMatch(name, _exceptions[i]))
                        return true;
                }
            }
           
            for (int i = 0; i <  AssetRuleList.instance.globalIgnoredPaths.Count; i++)
            {
                if (AssetRuleList.instance.globalIgnoredPaths[i].disable )
                {
                    if (Regex.IsMatch(name,  AssetRuleList.instance.globalIgnoredPaths[i].path))
                    {
                        return true;
                    }
                }
            }

            if (_pathRequirements != null)
            {
                for (int i = 0; i < _pathRequirements.Length; i++)
                {
                    if (Regex.IsMatch(name, _pathRequirements[i]) == false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public virtual bool DoesRuleApply(Object asset, string assetPath)
        {
            bool shouldIgnore = ShouldIgnore(assetPath);
            return shouldIgnore == false;
        }
    }
}