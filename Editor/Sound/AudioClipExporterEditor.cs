using System;
using System.Collections.Generic;
using GiantSword;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace GiantSwordEditor
{
    [CustomEditor(typeof(AudioClipExporter))]
    public class AudioClipExporterEditor : CustomEditorBase<AudioClipExporter>
    {

        [MenuItem("Assets/Trim AudioClip", true)]
        public static bool TrimAudioClipAssetMenuItemValidation(MenuCommand command)
        {
            AudioClip clip = Selection.activeObject as AudioClip;
            return clip != null;
        }
        
        [MenuItem("Assets/Trim Audio Clip")]
        public static void TrimAudioClipAssetMenuItem( MenuCommand command)
        {
            AudioClip clip = Selection.activeObject as AudioClip;
            
            if(clip == null)
            {
                return;
            }
            
            GameObject gameObject = new GameObject("TrimAudioClip " + clip.name + " (Not Saved)");
            gameObject.hideFlags |= HideFlags.DontSave;
            PlayableDirector playableDirector = gameObject.AddComponent<PlayableDirector>();
            playableDirector.playableAsset = ScriptableObject.CreateInstance<TimelineAsset>();
            
            TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
            
            var audioTrack = timelineAsset.CreateTrack<AudioTrack>(null, "Audio Track");

            var audioPlayableClip = audioTrack.CreateClip(clip);
            audioPlayableClip.displayName = " ";

            AudioClipExporter audioClipExporter = gameObject.AddComponent<AudioClipExporter>();
            RuntimeEditorHelper.SelectAndFocus(audioClipExporter);
            
            // RuntimeEditorHelper.EditorApplicationDelayCall(() => LockInspector());
            OpenTimelineWindow();

        }

        private static void OpenTimelineWindow()
        {
            EditorWindow.GetWindow(Type.GetType("UnityEditor.Timeline.TimelineWindow,Unity.Timeline.Editor"));
        }
        
        private static void LockInspector()
        {
            ActiveEditorTracker.sharedTracker.isLocked = true;
            Debug.Log("Inspector is locked " + ActiveEditorTracker.sharedTracker);
            ActiveEditorTracker.sharedTracker.ForceRebuild();
            
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // if (GUILayout.Button("Lock", GUILayout.Height(40)))
            // {
            //     LockInspector();
            // }
            
            if (GUILayout.Button("Export Clips", GUILayout.Height(40) ))
            {
                ShowConfirmationWindow();
            }
        }

        public void ShowConfirmationWindow()
        {
            ConfirmationWindow window = (ConfirmationWindow)EditorWindow.GetWindow(typeof(ConfirmationWindow));
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 250);
            window.SetAudioClipExporter(targetObject);
            window.ShowModalUtility();
        }

        public class ConfirmationWindow : EditorWindow
        {
            public static Preference<bool> deleteOriginalClips = new Preference<bool>("DeleteOriginalClips", false);
            public static Preference<bool> deleteTimelineSession = new Preference<bool>("DeleteTimelineSession", true);

            private AudioClipExporter _audioClipExporter;
            private string clipNames;
            private string folder;
            private List<AudioPlayableAsset> _audioPlayableAssets;

            private void OnGUI()
            {
                // EditorGUILayout.LabelField("Clip Names:");
                clipNames = EditorGUILayout.TextField("Clip Names:", clipNames);

                // GUILayout.Space(15);
                GUILayout.Space(15);

                for (var index = 0; index < _audioPlayableAssets.Count; index++)
                {
                    var audioPlayableAsset = _audioPlayableAssets[index];

                    string newPath = AudioClipExporterUtility.GetNewPath(folder, clipNames, index);
                    EditorGUILayout.LabelField(
                        $"{index}. {newPath}");
                }

                GUILayout.Space(15);

                deleteOriginalClips.value = GUILayout.Toggle(deleteOriginalClips.value, "Delete Original Clips");
                deleteTimelineSession.value =
                    GUILayout.Toggle(deleteTimelineSession.value, "Delete Timeline Session");
                GUILayout.Space(15);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Export", GUILayout.Height(40)))
                {
                    AudioClipExporterUtility.ExportAudioClips(_audioClipExporter.GetComponent<PlayableDirector>(),
                        folder,clipNames);

                  
                    Close();
                }
            }

            public string GetAssetFolderPath(Object asset)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                if (System.IO.File.Exists(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }
                return path;
            }

            public void SetAudioClipExporter(AudioClipExporter audioClipExporter)
            {
                _audioClipExporter = audioClipExporter;
                _audioPlayableAssets = _audioClipExporter.GetAudioPlayableAssets();
                if (_audioPlayableAssets.Count > 0)
                {
                    clipNames = _audioPlayableAssets[0].clip.name;
                    folder = GetAssetFolderPath(_audioPlayableAssets[0].clip);

                }
            }
        }
    }
}
