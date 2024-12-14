using UnityEngine;

namespace GiantSword.AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.ASSET_MENU_PATH + "/Rule for System Type ")]
    public class RuleForSystemType : RuleBase
    {
        public override bool DoesRuleApply(Object asset, string assetPath)
        {
            return asset.GetType() == type && base.DoesRuleApply(asset, assetPath);
        }
    }
}