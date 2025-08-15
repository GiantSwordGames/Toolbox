using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

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
        public enum Mode
        {
            RandomBag,
            PureRandom,
            Sequential,
            IndexedVariant,

            
        }
        
        [SerializeField] private Mode _mode = Mode.RandomBag;
        [SerializeField] private int _variantIndex = 0;

        public Playback _playback = Playback.OneShot;

        [Tooltip("2D -> 3D")] [Range(0, 1)]
        [SerializeField] public float _spacialBlend = 1f;
        [SerializeField] private AudioRolloffMode _rolloffMode = AudioRolloffMode.Linear;
        [SerializeField] private FloatRange _rolloffDistance = new FloatRange(1f, 25f);
        [SerializeField]  private AudioClip[] _clips;

        [FormerlySerializedAs("volume")]   [SerializeField] private FloatVariance _volume = new FloatVariance(0.5f, 0.05f);
        [FormerlySerializedAs("pitch")]  [SerializeField] private FloatVariance _pitch = new FloatVariance(1f, 0.05f);
        [SerializeField] private float _incrementalPitch = 0;
        [SerializeField] private float _incrementalPitchTimeout = 2;
        [FormerlySerializedAs("mixerGroup")]  [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private bool _useScaledTime = true;
        [SerializeField] private float _cooldown = 0;
        [FormerlySerializedAs("_pitchIncrement")] [FormerlySerializedAs("_frequecyIncrementRange")] [SerializeField] private float _pitchIncrementRange = 0;

        [SerializeField] FloatRange _velocityAttenuation = new FloatRange(0, 0);

        int _lastPlayedIndex = -1;
        [ShowNonSerializedField] private float _accumulatedPitch = 0;
        [SerializeField] private bool _debugLog = false;

        public bool isOneShot => _playback == Playback.OneShot;
        public bool isLooping => _playback == Playback.Loop;

        public AudioMixerGroup mixerGroup => _mixerGroup;
        public AudioRolloffMode rolloffMode => _rolloffMode;
        public FloatRange rolloffDistance => _rolloffDistance;
        public bool debugLog => _debugLog;

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
        public float cooldown => _cooldown;

        public float pitchIncrementRange => _pitchIncrementRange;

        public FloatRange velocityAttenuation => _velocityAttenuation;

        public Mode mode => _mode;

        public float incrementalPitchTimeout => _incrementalPitchTimeout;

        public float incrementalPitch => _incrementalPitch;
        public float dopplerLevel => 0;

        public AudioClip NextClip()
        {
            if (_clips == null || _clips.Length == 0)
            {
                Debug.LogWarning("No audio clips available in SoundAsset.");
                return null;
            }

            switch (_mode)
            {
                case Mode.RandomBag:
                    return _clips[Random.Range(0, _clips.Length)]; // not correctly implement
                case Mode.PureRandom:
                    return _clips[Random.Range(0, _clips.Length)];
                case Mode.Sequential:
                    return _clips[_lastPlayedIndex++%clips.Length];
                case Mode.IndexedVariant:
                    return _clips[Mathf.Clamp( _variantIndex, 0,clips.Length)];
                default:
                    return _clips[Random.Range(0, _clips.Length)];;
            }
            return null;
        }

        public void Play()
        {
            
        }

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
            // this.Play();
        }

        public void AssignDefaultValues()
        {
            _spacialBlend = DefaultSpatialBlend.value;
        }

        public void CreateNestedTimelineAsset()
        {
#if UNITY_EDITOR
            TimelineAsset asset = ScriptableObject.CreateInstance<TimelineAsset>();
            asset.name = "AudioTimeline";
           
            AudioTrack audioTrack = asset.CreateTrack<AudioTrack>(null, "Audio Track");

            // Add all audio clips to the AudioTrack
            foreach (var clip in _clips)
            {
                if (clip != null)
                {
                    var timelineClip = audioTrack.CreateClip<AudioPlayableAsset>();
                    var audioPlayableAsset = timelineClip.asset as AudioPlayableAsset;
                    audioPlayableAsset.clip = clip;
                    timelineClip.displayName = clip.name;
                }
            }
            AssetDatabase.AddObjectToAsset(asset, this);
           
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public float GetIncrementedPitch()
        {
            if (_incrementalPitchTimeout > 0)
            {

                if (TimeSinceLastPlay() > _incrementalPitchTimeout)
                {
                    _accumulatedPitch = 0;
                }

            }

            return _accumulatedPitch;
        }

        public void IncrementPitch()
        {
            _accumulatedPitch += _incrementalPitch;
        }
        

        public float TimeSinceLastPlay()
        {
            // float timeSinceLastPlay = Time.time - SoundInstanceManager.GetLastPlayTime(this);
            return 0;
        }
        
        
    }
}
