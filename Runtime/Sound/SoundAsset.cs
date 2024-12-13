using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class SoundAsset : ScriptableObject
    {
        
        public static readonly Preference<float> DefaultSpatialBlend = new Preference<float>("Spatial Blend",1f);
        public static readonly Preference<AudioMixerGroup> DefaultMixerGroup = new Preference<AudioMixerGroup>("Default Mixer",null);

        public enum Playback
        {
            OneShot,
            Loop,
        }

        public Playback _playback = Playback.OneShot;

        [Tooltip("2D -> 3D")] [Range(0, 1)]
        [SerializeField] public float _spacialBlend = 1f;
        [SerializeField] private AudioRolloffMode _rolloffMode = AudioRolloffMode.Logarithmic;
        [SerializeField] private FloatRange _rolloffDistance = new FloatRange(1f, 25f);
        [SerializeField]  private AudioClip[] _clips;

        [FormerlySerializedAs("volume")]   [SerializeField] private FloatVariance _volume = new FloatVariance(0.5f, 0.05f);
        [FormerlySerializedAs("pitch")]  [SerializeField] private FloatVariance _pitch = new FloatVariance(1f, 0.05f);

        [FormerlySerializedAs("mixerGroup")]  [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private bool _useScaledTime = true;

        public bool isOneShot => _playback == Playback.OneShot;
        public bool isLooping => _playback == Playback.Loop;

        public AudioMixerGroup mixerGroup => _mixerGroup;
        public AudioRolloffMode rolloffMode => _rolloffMode;
        public FloatRange rolloffDistance => _rolloffDistance;

        public FloatVariance volume
        {
            get => _volume;
            set => _volume = value;
        }

        public FloatVariance pitch => _pitch;
        public bool useScaledTime => _useScaledTime;

        public AudioClip[] clips
        {
            get => _clips;
            set => _clips = value;
        }

        public float spacialBlend => _spacialBlend;

        public AudioClip NextClip() => _clips?.Length > 0 ? _clips[Random.Range(0, _clips.Length)] : null;

        public void AddClips(List<AudioClip> newClips)
        {
            if (newClips == null || newClips.Count == 0)
            {
                Debug.LogWarning("Attempting to add an empty or null list of clips.");
                return;
            }

            HashSet<AudioClip> uniqueClips = new HashSet<AudioClip>(_clips);
            uniqueClips.UnionWith(newClips);
            _clips = new AudioClip[uniqueClips.Count];
            uniqueClips.CopyTo(_clips);
        }

        private const string AUDIO_CLIP_DESTINATION_PATH = "Assets/Project/Audio/Clips/";

        public void ImportAudioClips()
        {
            #if UNITY_EDITOR
            if (_clips == null || _clips.Length == 0)
            {
                Debug.LogWarning("No audio clips available in SoundAsset.");
                return;
            }

            // Ensure the destination folder exists
            if (!Directory.Exists(AUDIO_CLIP_DESTINATION_PATH))
            {
                Directory.CreateDirectory(AUDIO_CLIP_DESTINATION_PATH);
                AssetDatabase.Refresh();  // Refresh AssetDatabase to recognize the new directory
            }

            for (int i = 0; i < _clips.Length; i++)
            {
                var clip = _clips[i];
                if (clip == null) continue;

                string assetPath = AssetDatabase.GetAssetPath(clip);

                // Check if the clip is outside the project
                if (!assetPath.StartsWith("Assets/Project"))
                {
                    // Copy the audio clip to the project directory
                    string sourcePath = Path.GetFullPath(assetPath);
                    string destinationPath = Path.Combine(AUDIO_CLIP_DESTINATION_PATH, Path.GetFileName(sourcePath));

                    // Copy the file and refresh the AssetDatabase
                    File.Copy(sourcePath, destinationPath, true);
                    AssetDatabase.Refresh();

                    // Load the copied clip into the project
                    _clips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(destinationPath);

                    RuntimeEditorHelper.SetDirty(this);
                    Debug.Log($"Copied {clip.name} to {destinationPath}",_clips[i] );
                }
            }
            #endif
        }
        
        
        public void Trigger()
        {
            this.Play();
        }

        public void AssignDefaultValues()
        {
            _spacialBlend = DefaultSpatialBlend.value;
        }
    }
}
