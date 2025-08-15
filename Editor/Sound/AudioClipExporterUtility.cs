using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JamKitEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace JamKit
{
    public static class AudioClipExporterUtility
    {
        // Added a new bool parameter: overwriteOriginal
        public static void ExportAudioClips(PlayableDirector playableDirector, string outputPath, string newName, bool overwriteOriginal)
        {
            if (playableDirector == null)
            {
                Debug.LogError("PlayableDirector is not assigned.");
                return;
            }

            // Ensure the output path exists
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                Debug.Log($"Created output directory at: {Path.GetFullPath(outputPath)}");
            }

            // Get the Timeline asset
            var timelineAsset = playableDirector.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                Debug.LogError("PlayableDirector does not have a TimelineAsset.");
                return;
            }

            IEnumerable<TrackAsset> trackAssets = timelineAsset.GetOutputTracks();
            List<AudioPlayableAsset> audioPlayableAssets = new List<AudioPlayableAsset>();
            List<string> exportedPaths = new List<string>();

            // Iterate through all output tracks in the Timeline
            foreach (var track in trackAssets)
            {
                if (track is AudioTrack audioTrack)
                {
                    List<TimelineClip> timelineClips = audioTrack.GetClips().ToList();
                    for (var index = 0; index < timelineClips.Count; index++)
                    {
                        var clip = timelineClips[index];
                        var audioPlayableAsset = clip.asset as AudioPlayableAsset;
                        if (audioPlayableAsset != null)
                        {
                            // Add asset to list for later processing (e.g. deletion)
                            audioPlayableAssets.Add(audioPlayableAsset);

                            AudioClip originalClip = audioPlayableAsset.clip;
                            if (originalClip == null)
                            {
                                Debug.LogWarning(
                                    $"AudioPlayableAsset in clip '{clip.displayName}' has no AudioClip assigned.");
                                continue;
                            }

                            // Get the start time and duration of the clip within the Timeline
                            double startTime = clip.clipIn;
                            double endTime = clip.clipIn + clip.duration;

                            // Trim the audio clip based on the Timeline's clip
                            AudioClip trimmedClip = TrimAudioClip(originalClip, (float)startTime, (float)endTime);
                            if (trimmedClip == null)
                            {
                                Debug.LogWarning($"Failed to trim AudioClip '{originalClip.name}'.");
                                continue;
                            }

                            // Determine the filename:
                            string filename;
                            // If overwrite option is true and we only have one clip, use the original asset's path.
                            if (overwriteOriginal && audioPlayableAssets.Count == 1)
                            {
                                string originalAssetPath = AssetDatabase.GetAssetPath(originalClip);
                                if (!string.IsNullOrEmpty(originalAssetPath))
                                {
                                    filename = originalAssetPath;
                                }
                                else
                                {
                                    filename = GetNewPath(outputPath, newName, index);
                                }
                            }
                            else
                            {
                                filename = GetNewPath(outputPath, newName, index);
                            }

                            exportedPaths.Add(filename);
                            // Save the trimmed clip as a WAV file
                            SaveWavFile(trimmedClip, filename);
                        }
                    }
                }
            }

            // Import exported files
            foreach (var exportedPath in exportedPaths)
            {
                AssetDatabase.ImportAsset(exportedPath);
            }

            if (AudioClipExporterEditor.ConfirmationWindow.deleteOriginalClips)
            {
                List<AudioClip> clips = new List<AudioClip>();
                foreach (var asset in audioPlayableAssets)
                {
                    if (!clips.Contains(asset.clip))
                    {
                        clips.Add(asset.clip);
                    }
                }

                foreach (AudioClip clip in clips)
                {
                    Debug.Log("Deleting " + clip.name);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(clip));
                }
            }

            if (AudioClipExporterEditor.ConfirmationWindow.deleteTimelineSession)
            {
                Undo.DestroyObjectImmediate(playableDirector.gameObject);
                RuntimeEditorHelper.EditorApplicationDelayCall(() =>
                    Selection.activeObject = GameObject.FindObjectOfType<Transform>());
                Debug.Log("Selection.activeObject " + Selection.activeObject, Selection.activeObject);
            }

            // Select the exported files in the Project window
            Selection.objects = exportedPaths.ConvertAll(path => AssetDatabase.LoadAssetAtPath<AudioClip>(path)).ToArray();
            Debug.Log("Export completed. " + audioPlayableAssets.Count);
        }

        public static string GetNewPath(string outputPath, string originalClipName, int index)
        {
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string filename;
            string letter = index.ToString();
            if (index < letters.Length)
            {
                letter = letters[index].ToString();
            }
            filename = Path.Combine(outputPath, $"{originalClipName}_{letter}.wav");
            return filename;
        }

        private static AudioClip TrimAudioClip(AudioClip clip, float startTime, float endTime)
        {
            // Calculate sample positions
            int frequency = clip.frequency;
            int channels = clip.channels;
            int startSample = Mathf.FloorToInt(startTime * frequency * channels);
            int endSample = Mathf.FloorToInt(endTime * frequency * channels);

            // Get original audio data
            float[] originalData = new float[clip.samples * channels];
            clip.GetData(originalData, 0);

            // Calculate length of the new clip
            int trimmedLength = endSample - startSample;

            // Create a new AudioClip with the trimmed length
            AudioClip trimmedClip = AudioClip.Create(clip.name + "_trimmed", trimmedLength / channels, channels, frequency, false);

            // Extract the relevant audio data
            float[] trimmedData = new float[trimmedLength];
            Array.Copy(originalData, startSample, trimmedData, 0, trimmedLength);

            // Set the data for the new clip
            trimmedClip.SetData(trimmedData, 0);
            return trimmedClip;
        }

        /// <summary>
        /// Saves an AudioClip as a WAV file to the specified filename.
        /// </summary>
        private static void SaveWavFile(AudioClip clip, string filename)
        {
            if (clip == null)
            {
                Debug.LogWarning("Cannot save a null AudioClip.");
                return;
            }

            // Calculate total number of samples (all channels)
            int totalSamples = clip.samples * clip.channels;
            float[] samples = new float[totalSamples];
            if (!clip.GetData(samples, 0))
            {
                Debug.LogWarning($"Failed to retrieve data from AudioClip '{clip.name}'.");
                return;
            }

            byte[] wavFile = ConvertToWav(samples, clip.channels, clip.frequency);
            if (wavFile == null || wavFile.Length == 0)
            {
                Debug.LogWarning($"Conversion to WAV failed for AudioClip '{clip.name}'.");
                return;
            }

            try
            {
                File.WriteAllBytes(filename, wavFile);
                Debug.Log($"Exported audio file: {Path.GetFullPath(filename)}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write WAV file '{filename}': {ex.Message}");
            }
        }

        /// <summary>
        /// Converts an array of float samples to a WAV formatted byte array.
        /// </summary>
        private static byte[] ConvertToWav(float[] samples, int channels, int frequency)
        {
            if (samples == null || samples.Length == 0)
            {
                Debug.LogWarning("No samples provided for WAV conversion.");
                return null;
            }

            int bitsPerSample = 16;
            int bytesPerSample = bitsPerSample / 8;
            int byteRate = frequency * channels * bytesPerSample;
            int blockAlign = channels * bytesPerSample;
            int dataSize = samples.Length * bytesPerSample;
            int fileSize = 44 + dataSize;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    // RIFF header
                    writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                    writer.Write(fileSize - 8);
                    writer.Write(new char[4] { 'W', 'A', 'V', 'E' });

                    // fmt subchunk
                    writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                    writer.Write(16);
                    writer.Write((short)1);
                    writer.Write((short)channels);
                    writer.Write(frequency);
                    writer.Write(byteRate);
                    writer.Write((short)blockAlign);
                    writer.Write((short)bitsPerSample);

                    // data subchunk
                    writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                    writer.Write(dataSize);

                    // Write audio data
                    foreach (var sample in samples)
                    {
                        short pcmSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                        writer.Write(pcmSample);
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
