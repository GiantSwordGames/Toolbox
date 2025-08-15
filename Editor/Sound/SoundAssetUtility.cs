using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JamKit
{
    public static class SoundAssetUtility
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        private static void OnSceneSaved(Scene scene)
        {
            return;
            List<SoundAsset> soundAssets = RuntimeEditorHelper.FindAssets<SoundAsset>();
            foreach (var soundAsset in soundAssets)
            {
                soundAsset.ImportAudioClips();
            }
        }
    }
}