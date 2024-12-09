using UnityEngine;
using UnityEditor;

namespace GiantSword
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundAsset))]
    public class SoundAssetEditor : CustomEditorBase<SoundAsset>
    {
        private AudioSource _previewSource;
        private AudioClip _lastPlayedClip;
        private Texture2D _waveformTexture;

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            SoundAsset soundAsset = (SoundAsset)target;

            // Add a space and a "Play" button
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

            // Always draw the waveform of the first available clip, even if not playing
            if (soundAsset.clips != null && soundAsset.clips.Length > 0)
            {
                AudioClip clipToDraw = soundAsset.clips[0]; // Draw the first clip by default
                if (clipToDraw != null)
                {
                    GUILayout.Space(10);
                    DrawWaveform(clipToDraw);
                    GUILayout.BeginVertical( CreateDarkerBoxStyle());
                    DisplayHeader(clipToDraw);
                    DisplayDetails(clipToDraw);
                    GUILayout.EndVertical();
                }
            }
        }

        private GUIStyle CreateDarkerBoxStyle()
        {
            GUIStyle darkBoxStyle = new GUIStyle(GUI.skin.box);
            darkBoxStyle.normal.background = MakeTex(2, 2, new Color(0.15f, 0.29f, 0.33f)); // Dark color
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
        
        private void DisplayHeader(AudioClip clip)
        {
            GUILayout.BeginHorizontal();
            // Clip name
            GUILayout.Label($"{clip.name}");
            
            GUILayout.FlexibleSpace();
            
            // Clip length in seconds
            GUILayout.Label($"{clip.length:F2} s");
            GUILayout.EndHorizontal();
        }
        
        private void DisplayDetails(AudioClip clip)
        {
            GUILayout.BeginHorizontal();
            // Number of channels (mono/stereo)
            GUILayout.Label($"Ch: {clip.channels} {(clip.channels == 1 ? "(Mono)" : "(Stereo)")}");

            GUILayout.FlexibleSpace();

            // Bit rate calculation (approximation)
            int bitRate = Mathf.RoundToInt((clip.frequency * clip.channels * 16) / 1000f);
            GUILayout.Label($"{bitRate} kbps");
            GUILayout.FlexibleSpace();

            // Maximum dB level
            float maxDB = GetMaxDB(clip);
            GUILayout.Label($"Max dB: {maxDB:F2} dB");
            GUILayout.EndHorizontal();

        }

        private float GetMaxDB(AudioClip clip)
        {
            if (clip == null) return 0f; // Return 0 if no clip is provided

            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            float maxAmplitude = 0f;
            foreach (float sample in samples)
            {
                maxAmplitude = Mathf.Max(maxAmplitude, Mathf.Abs(sample));
            }

            // Convert amplitude to dB and return as a positive value
            float maxDB = 20 * Mathf.Log10(maxAmplitude);
            return Mathf.Clamp(Mathf.Abs(maxDB), 0f, 80f); // Ensure the result is positive and clamped between 0 dB and 80 dB
        }

        private void PlayAudioClip(SoundAsset soundAsset)
        {
            if (soundAsset.clips == null || soundAsset.clips.Length == 0)
            {
                Debug.LogWarning("No audio clips found in this SoundAsset.");
                return;
            }

            // Select a random clip from the sound asset
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

            // If we're in the editor and not in play mode, use a temporary AudioSource
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

                // Store the last played clip for waveform display and schedule cleanup
                _lastPlayedClip = clipToPlay;
                EditorApplication.update += RemovePreviewSource;
                Repaint(); // Force the inspector to repaint to show the playhead
            }
            else
            {
                // Use AudioSource.PlayClipAtPoint during play mode
                AudioSource.PlayClipAtPoint(clipToPlay, audioListener.transform.position, soundAsset.volume.GetRandom());

                // Store the last played clip for waveform display
                _lastPlayedClip = clipToPlay;
            }
        }

        private void DrawWaveform(AudioClip clip)
        {
            if (clip == null) return;

            // Ensure we have a texture to draw the waveform
            int width = 300;
            int height = 100;
            if (_waveformTexture == null || _waveformTexture.width != width)
            {
                _waveformTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                _waveformTexture.hideFlags = HideFlags.HideAndDontSave;
            }

            // Get the samples from the audio clip
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            // Clear the texture
            Color backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            Color waveformColor = Color.green;
            Color zeroLineColor = Color.red;
            Color playheadColor = Color.white;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _waveformTexture.SetPixel(x, y, backgroundColor);
                }
            }

            // Draw the zero line
            int zeroLineY = height / 2;
            for (int x = 0; x < width; x++)
            {
                _waveformTexture.SetPixel(x, zeroLineY, zeroLineColor);
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

                // Map the amplitude to the height of the texture
                int positiveY = zeroLineY + Mathf.RoundToInt((maxAmplitude * (height / 2)));
                int negativeY = zeroLineY - Mathf.RoundToInt((maxAmplitude * (height / 2)));

                // Draw positive and negative amplitude lines
                for (int y = zeroLineY; y <= positiveY; y++)
                {
                    _waveformTexture.SetPixel(x, y, waveformColor);
                }
                for (int y = zeroLineY; y >= negativeY; y--)
                {
                    _waveformTexture.SetPixel(x, y, waveformColor);
                }
            }

            // Draw the playhead marker if playing
            if (_previewSource != null && _previewSource.isPlaying)
            {
                float playheadPosition = (_previewSource.time / clip.length) * width;
                int playheadX = Mathf.Clamp(Mathf.RoundToInt(playheadPosition), 0, width - 1);

                for (int y = 0; y < height; y++)
                {
                    _waveformTexture.SetPixel(playheadX, y, playheadColor);
                }

                Repaint(); // Continuously repaint the inspector to update the playhead position
            }

            _waveformTexture.Apply();

            // Draw the texture in the inspector
            GUILayout.Label(_waveformTexture, GUILayout.Width(width), GUILayout.Height(height));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // Clean up the preview AudioSource when the inspector is closed
            if (_previewSource != null)
            {
                DestroyImmediate(_previewSource.gameObject);
                EditorApplication.update -= RemovePreviewSource;
            }

            if (_waveformTexture != null)
            {
                DestroyImmediate(_waveformTexture);
            }
        }

        private void RemovePreviewSource()
        {
            // Remove the preview source if it is not playing
            if (_previewSource != null && !_previewSource.isPlaying)
            {
                DestroyImmediate(_previewSource.gameObject);
                _previewSource = null;
                EditorApplication.update -= RemovePreviewSource;
            }
        }
    }
}
