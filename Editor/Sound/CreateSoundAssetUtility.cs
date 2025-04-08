using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace GiantSword
{

    public class DynamicContextMenuEditor : EditorWindow
    {
        private List<SoundAsset> _soundBanks;
        private List<AudioClip> _audioClips;

        [MenuItem(MenuPaths.QUICK_CREATE + "Assign To Sound Bank")]
        public static void OpenWindow()
        {
            var window = GetWindow<DynamicContextMenuEditor>("Assign Audio Clip to Sound Bank", true);
            window.ShowPopup();
        }

        private void Awake()
        {
            _soundBanks = RuntimeEditorHelper.FindAssetsOfType<SoundAsset>();
            _audioClips = Selection.objects.ExtractElementsOfType<AudioClip, Object>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Select a Sound Bank ");
            GUILayout.BeginVertical();
            foreach (SoundAsset soundBank in _soundBanks)
            {
                if (GUILayout.Button(soundBank.name))
                {
                    RuntimeEditorHelper.RecordObjectUndo(soundBank);
                    soundBank.AddClips(_audioClips);
                    Debug.Log("Added " + _audioClips.Count + " clips to " + soundBank.name,soundBank);
                    Close();
                }
            }
            GUILayout.EndVertical();
        }
        
    }

    public static class CreateSoundAssetUtility
    {

        [MenuItem(MenuPaths.QUICK_CREATE + "Create Sound Bank From Clips", false, MenuPaths.QUICK_CREATE_PRIORITY)]
        public static void CreateSoundAsset()
        {
            var clips = Selection.objects.ExtractElementsOfType<AudioClip, Object>();

            if (clips.Count == 0)
            {
                Debug.LogError("No Audio Clips Selected");
                return;
            }

            var newAsset = CreateSoundAssetForClips(null, clips.ToArray());

            RuntimeEditorHelper.SelectAndFocus(newAsset);
        }

        [MenuItem(MenuPaths.QUICK_CREATE + "Create Sound Banks From Clips", false, MenuPaths.QUICK_CREATE_PRIORITY)]
        public static void CreateSoundAssets()
        {
            var clips = Selection.objects.ExtractElementsOfType<AudioClip, Object>();

            if (clips.Count == 0)
            {
                Debug.LogError("No Audio Clips Selected");
                return;
            }

            List<SoundAsset> banks = new List<SoundAsset>();
            foreach (var clip in clips)
            {
                var newAsset = CreateSoundAssetForClips(null, clip);
                banks.Add(newAsset);
            }

            RuntimeEditorHelper.SelectAndFocus(banks);
        }

        public static SoundAsset CreateSoundBank(string name = null)
        {
            var sound = CreateSoundAssetForClips(name, null);
            RuntimeEditorHelper.SetDirty(sound);
            return sound;
        }

        private static SoundAsset CreateSoundAssetForClips(string name = null, params AudioClip[] clips)
        {
            SoundAsset soundAsset = ScriptableObject.CreateInstance<SoundAsset>();
            soundAsset.AssignDefaultValues();
            string folderPath = RuntimeEditorHelper.GetMostCommonDirectoryForAssetType<SoundAsset>();
            if (folderPath == "")
            {

                Directory.CreateDirectory(Application.dataPath + "/Project/Audio/SoundBanks");
                folderPath = "Assets/Project/Audio/SoundBanks";
                Debug.LogWarning("Creating Sound Bank Folder in Assets/Project/Audio/SoundBanks");
            }

            string assetName = "Sound_Untitled";
            if (clips != null)
            {
                soundAsset.clips = clips;
                assetName = "Sound_" + clips[0].name;
            }

            if (name != null)
            {
                assetName = "Sound_" + name;
            }

            string newPath = folderPath + "/" + assetName + ".asset";
            AssetDatabase.CreateAsset(soundAsset, newPath);
            var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<SoundAsset>(newPath);
            Debug.Log(newPath, loadAssetAtPath);

            return loadAssetAtPath;
        }
    }
}
