using UnityEngine;
using UnityEditor;

namespace GiantSword
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundAsset))]
    public class SoundAssetEditor : CustomEditorBase<SoundAsset>
    {
        private AudioSource _previewSource;
        private AudioClip _lastClip;
        private Texture2D _cachedWaveformTexture;
        private bool _displayDefaultSettings = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultSettings();
            DrawDefaultInspector();

            SoundAsset soundAsset = (SoundAsset)target;

            GUILayout.Space(10);
            if (GUILayout.Button("Play"))
            {
                PlayAudioClip(soundAsset);
            }

            if (GUILayout.Button("Import"))
            {
                foreach (SoundAsset asset in targetObjects)
                {
                    asset.ImportAudioClips();
                }
            }
            
            if (GUILayout.Button("CreateTimeline"))
            {
                foreach (SoundAsset asset in targetObjects)
                {
                    asset.CreateNestedTimelineAsset();
                }
            }

            if (soundAsset.clips != null && soundAsset.clips.Length > 0)
            {
                AudioClip clipToDraw = soundAsset.clips[0]; // Draw the first clip by default
                if (clipToDraw != null)
                {
                    GUILayout.Space(10);
                    DrawWaveform(clipToDraw);
                    GUILayout.BeginVertical(CreateDarkerBoxStyle());
                    DisplayHeader(clipToDraw);
                    DisplayDetails(clipToDraw);
                    GUILayout.EndVertical();
                }
            }
        }

        private void DrawDefaultSettings()
        {
            _displayDefaultSettings = EditorGUILayout.Foldout(_displayDefaultSettings, "Default Settings");

            if (_displayDefaultSettings)
            {
                EditorGUI.indentLevel++;
                SoundAsset.DefaultSpatialBlend.DrawSlider(0, 1);
                SoundAsset.DefaultMixerGroup.DrawDefaultGUI();
                EditorGUI.indentLevel--;
            }
        }

        private void DrawWaveform(AudioClip clip)
        {
            if (clip == null) return;

            // Regenerate the cached texture if the clip has changed
            if (_lastClip != clip || _cachedWaveformTexture == null)
            {
                _cachedWaveformTexture = GenerateWaveformTexture(clip, 300, 100);
                _lastClip = clip;
            }

            Rect waveformRect = GUILayoutUtility.GetRect(_cachedWaveformTexture.width, _cachedWaveformTexture.height);
            GUI.DrawTexture(waveformRect, _cachedWaveformTexture);

            // Draw the playhead line if the audio is playing
            if (_previewSource != null && _previewSource.isPlaying)
            {
                float playheadPosition = (_previewSource.time / clip.length) * waveformRect.width;
                DrawPlayheadLine(waveformRect, playheadPosition);
                Repaint(); // Force repaint to update playhead in real time
            }
        }

        private void DrawPlayheadLine(Rect waveformRect, float playheadX)
        {
            Color playheadColor = Color.red;
            float lineX = waveformRect.xMin + playheadX;

            Handles.color = playheadColor;
            Handles.DrawLine(
                new Vector3(lineX, waveformRect.yMin),
                new Vector3(lineX, waveformRect.yMax)
            );
        }

        private Texture2D GenerateWaveformTexture(AudioClip clip, int width, int height)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;

            Color backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            Color waveformColor = Color.green;
            int zeroLineY = height / 2;

            // Clear the texture
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, backgroundColor);
                }
            }

            // Draw the waveform
            int sampleStep = Mathf.Max(1, samples.Length / width);
            for (int x = 0; x < width; x++)
            {
                float maxAmplitude = 0f;
                for (int i = 0; i < sampleStep; i++)
                {
                    int sampleIndex = x * sampleStep + i;
                    if (sampleIndex < samples.Length)
                    {
                        float sampleValue = samples[sampleIndex];
                        maxAmplitude = Mathf.Max(maxAmplitude, Mathf.Abs(sampleValue));
                    }
                }

                int positiveY = zeroLineY + Mathf.RoundToInt(maxAmplitude * (height / 2));
                int negativeY = zeroLineY - Mathf.RoundToInt(maxAmplitude * (height / 2));

                for (int y = zeroLineY; y <= positiveY; y++)
                {
                    texture.SetPixel(x, y, waveformColor);
                }
                for (int y = zeroLineY; y >= negativeY; y--)
                {
                    texture.SetPixel(x, y, waveformColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private void DisplayHeader(AudioClip clip)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{clip.name}");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{clip.length:F2} s");
            GUILayout.EndHorizontal();
        }

        private void DisplayDetails(AudioClip clip)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Ch: {clip.channels} {(clip.channels == 1 ? "(Mono)" : "(Stereo)")}");
            GUILayout.FlexibleSpace();

            int bitRate = Mathf.RoundToInt((clip.frequency * clip.channels * 16) / 1000f);
            GUILayout.Label($"{bitRate} kbps");
            GUILayout.FlexibleSpace();

            float maxDB = GetMaxDB(clip);
            GUILayout.Label($"Max dB: {maxDB:F2} dB");
            GUILayout.EndHorizontal();
        }

        private float GetMaxDB(AudioClip clip)
        {
            if (clip == null) return 0f;

            const int sampleCount = 128; // Number of samples to evaluate
            float[] samples = new float[sampleCount];
            int totalSamples = clip.samples * clip.channels;
            int stepSize = Mathf.Max(1, totalSamples / sampleCount);

            float maxAmplitude = 0f;

            // Retrieve evenly spaced samples
            for (int i = 0; i < sampleCount; i++)
            {
                int sampleIndex = i * stepSize;
                if (sampleIndex < totalSamples)
                {
                    clip.GetData(samples, sampleIndex / clip.channels);
                    for (int j = 0; j < clip.channels; j++)
                    {
                        maxAmplitude = Mathf.Max(maxAmplitude, Mathf.Abs(samples[j]));
                    }
                }
            }

            // Convert amplitude to decibels
            float maxDB = 20 * Mathf.Log10(maxAmplitude);
            return Mathf.Clamp(Mathf.Abs(maxDB), 0f, 80f);
        }

        private void PlayAudioClip(SoundAsset soundAsset)
        {
            if (soundAsset.clips == null || soundAsset.clips.Length == 0)
            {
                Debug.LogWarning("No audio clips found in this SoundAsset.");
                return;
            }

            AudioClip clipToPlay = soundAsset.NextClip();
            if (clipToPlay == null)
            {
                Debug.LogWarning("The selected clip is null.");
                return;
            }

            AudioListener audioListener = FindObjectOfType<AudioListener>();
            if (audioListener == null)
            {
                Debug.LogWarning("No AudioListener found in the scene.");
                return;
            }

            if (!Application.isPlaying)
            {
                if (_previewSource == null)
                {
                    GameObject tempGO = new GameObject("AudioPreview");
                    tempGO.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;

                    _previewSource = tempGO.AddComponent<AudioSource>();
                    _previewSource.hideFlags = HideFlags.HideAndDontSave;
                    tempGO.transform.position = audioListener.transform.position;
                }

                _previewSource.clip = clipToPlay;
                _previewSource.volume = soundAsset.volume.GetRandom();
                _previewSource.pitch = soundAsset.pitch.GetRandom();
                _previewSource.spatialBlend = soundAsset.spacialBlend;
                _previewSource.outputAudioMixerGroup = soundAsset.mixerGroup;
                _previewSource.Play();

                _lastClip = clipToPlay;
                EditorApplication.update += RemovePreviewSource;
                Repaint();
            }
            else
            {
                AudioSource.PlayClipAtPoint(clipToPlay, audioListener.transform.position, soundAsset.volume.GetRandom());
                _lastClip = clipToPlay;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_previewSource != null)
            {
                DestroyImmediate(_previewSource.gameObject);
                EditorApplication.update -= RemovePreviewSource;
            }

            if (_cachedWaveformTexture != null)
            {
                DestroyImmediate(_cachedWaveformTexture);
                _cachedWaveformTexture = null;
            }
        }

        private void RemovePreviewSource()
        {
            if (_previewSource != null && !_previewSource.isPlaying)
            {
                DestroyImmediate(_previewSource.gameObject);
                _previewSource = null;
                EditorApplication.update -= RemovePreviewSource;
            }
        }

        private GUIStyle CreateDarkerBoxStyle()
        {
            GUIStyle darkBoxStyle = new GUIStyle(GUI.skin.box);
            darkBoxStyle.normal.background = MakeTex(2, 2, new Color(0.15f, 0.29f, 0.33f));
            return darkBoxStyle;
        }

        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
