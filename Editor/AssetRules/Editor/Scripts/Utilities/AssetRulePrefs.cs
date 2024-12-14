using GiantSword;
using GiantSword;
using UnityEditor;
using UnityEngine;

namespace GiantSword.AssetRules
{
    public static class AssetRulePreferences
    {
        private static Preference<bool> _checkRulesOnAssetImport = new Preference<bool>("_CheckNamingRulesOnAssetImport", false);
        private static Preference<bool> _autoApplyNamingConventions = new Preference<bool>("_autoApplyNamingConventions", false);

        public static bool checkRulesOnAssetImport => _checkRulesOnAssetImport.value;
        public static Preference<bool> autoApplyNamingConventions => _autoApplyNamingConventions;

        [InitializeOnLoadMethod]
        public static void InitializeSettings()
        {
            DeveloperPreferences.RegisterSettingDrawer(new DeveloperPreferences.SettingDrawer()
            {
                keywords = new[] { "Asset Rules" },
                onGUI = ()=>
                {
                    _checkRulesOnAssetImport.value = GUILayout.Toggle(_checkRulesOnAssetImport.value, "Check Naming Conventions On Asset Import");
                    autoApplyNamingConventions.value = GUILayout.Toggle(autoApplyNamingConventions.value, "Auto Apply Naming Conventions On Asset Import");
                }
            });
        }
    }
}