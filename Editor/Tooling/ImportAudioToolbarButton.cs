using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    public static class ImportAudioToolbarButton
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
            Texture texture = RuntimeEditorHelper.FindAssetByName<Texture>("Icon_ImportAudioFromDownloads");
            _guiContent = new GUIContent(texture, "Import Audio From Downloads");
            if(GUILayout.Button( _guiContent,layoutWidth, layoutHeight))
            {
                ImportAudioFromDownloads.CopyAudioFilesFromDownloadsToAssets();
                Debug.Log("Import Audio From Downloads");
            }
        }
    }
}
