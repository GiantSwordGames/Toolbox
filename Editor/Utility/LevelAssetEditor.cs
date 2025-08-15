using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public class LevelAssetUtility
    {
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // Check if the asset being opened is a Level asset
            Level level = EditorUtility.InstanceIDToObject(instanceID) as Level;
            if (level != null)
            {
                level.OpenLevel();
                return true; // Return true to indicate the asset open has been handled
            }
            return false; // Return false to let Unity handle other assets as normal
        }
        
        
        private static List<Level> _levels;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            UnityToolbarExtender.rightOfPlayButton.Add(OpenLevel);
        }
        
        private static void OpenLevel()
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("Open Level"), FocusType.Passive))
            {
                _levels = RuntimeEditorHelper.FindAssetsOfType<Level>();
                // create dropdown menu
                GenericMenu menu = new GenericMenu();
                foreach (Level level in _levels)
                {
                    menu.AddItem(new GUIContent(level.name), false, () => level.OpenLevel());
                }
                menu.ShowAsContext();
            }
        }
    }
}
