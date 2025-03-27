using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    public static class BuildWindowsAndMacToolBarButton
    {
        private static GUIContent _guiContent;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {  
            UnityToolbarExtender.farRight.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayoutOption layoutWidth = GUILayout.Width( 26);
            GUILayoutOption layoutHeight = GUILayout.Height( 19);
            Texture texture = RuntimeEditorHelper.FindAssetByName<Texture>("Icon_BuildWinMac");
            _guiContent = new GUIContent(texture, "Build Windows And Mac");
            if(GUILayout.Button( _guiContent,layoutWidth, layoutHeight))
            {
                BuildAndZip.BuildAllPlatforms();
            }
        }
    }
    
    public static class BuildPreferences
    {

        [InitializeOnLoadMethod]
        public static void InitializeSettings()
        {
            DeveloperPreferences.RegisterSettingDrawer(new DeveloperPreferences.SettingDrawer()
            {
                keywords = new[] { "Build" },
                onGUI = ()=>
                {
                    GUILayout.Label("Build Button");
                    BuildAndZip.buildForMac.DrawDefaultGUI();
                    BuildAndZip.buildForWindows.DrawDefaultGUI();
                    BuildAndZip.buildForWebGL.DrawDefaultGUI();
                    BuildAndZip.buildForLinux.DrawDefaultGUI();
                }
            });
        }

       
    }
}