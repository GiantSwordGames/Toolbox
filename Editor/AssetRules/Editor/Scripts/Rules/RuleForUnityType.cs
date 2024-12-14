using UnityEditor;
using UnityEngine;

namespace GiantSword.AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.ASSET_MENU_PATH +"/Rule For Unity Type" )]
    public class RuleForUnityType : RuleBase
    {
        public PrefabAssetType prefabType;
        [SerializeField] private string[] requiredComponents = {};
        
        public override bool DoesRuleApply(Object asset, string assetPath)
        {
            if (base.DoesRuleApply(asset, assetPath) == false)
                return false;
            
            if (asset is GameObject go)
            {
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(asset);

                for (int i = 0; i < requiredComponents.Length; i++)
                {
                    Component component = go.GetComponent(requiredComponents[i]);
                    if (component == null)
                    {
                        return false;
                    }
                }
                
                    
                return prefabAssetType == prefabType;
            }

            return false;
        }
    }
}