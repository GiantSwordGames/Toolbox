
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
        public static void ExportAudioClips(
            PlayableDirector playableDirector,
            string outputPath,
            string newName,
            bool overwriteOriginal)
        {
            if (playableDirector == null)
            {
                Debug.LogError("PlayableDirector is not assigned.");
                return;
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                Debug.Log($"Created output directory at: {Path.GetFullPath(outputPath)}");
            }

            if (!(playableDirector.playableAsset is TimelineAsset timelineAsset))
            {
                Debug.LogError("PlayableDirector does not have a TimelineAsset.");
                return;
            }

            // Collect all audio assets
            List<AudioPlayableAsset> allAudioAssets = new List<AudioPlayableAsset>();
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is AudioTrack audioTrack)
                {
                    allAudioAssets.AddRange(
                        audioTrack.GetClips()
                            .Select(c => c.asset as AudioPlayableAsset)
                            .Where(a => a != null));
                }
            }

            bool canOverwrite = overwriteOriginal && allAudioAssets.Count == 1;
            List<string> exportedPaths = new List<string>();

            for (int i = 0; i < allAudioAssets.Count; i++)
            {
                var audioPlayableAsset = allAudioAssets[i];
                var clip = audioPlayableAsset.clip;
                if (clip == null) continue;

                var timelineClip = timelineAsset
                    .GetOutputTracks()
                    .OfType<AudioTrack>()
                    .SelectMany(t => t.GetClips())
                    .FirstOrDefault(c => c.asset == audioPlayableAsset);

                if (timelineClip == null) continue;

                double startTime = timelineClip.clipIn;
                double endTime = timelineClip.clipIn + timelineClip.duration;

                AudioClip trimmedClip = TrimAudioClip(clip, (float)startTime, (float)endTime);
                if (trimmedClip == null)
                {
                    Debug.LogWarning($"Failed to trim AudioClip '{clip.name}'.");
                    continue;
                }

                string filename;
                if (canOverwrite)
                {
                    string originalAssetPath = AssetDatabase.GetAssetPath(clip);
                    if (!string.IsNullOrEmpty(originalAssetPath))
                    {
                        string ext = Path.GetExtension(originalAssetPath);
                        filename = Path.ChangeExtension(originalAssetPath, ext);
                    }
                    else
                    {
                        filename = GetNewPath(outputPath, newName, i);
                    }
                }
                else
                {
                    filename = GetNewPath(outputPath, newName, i);
                }

                exportedPaths.Add(filename);
                SaveWavFile(trimmedClip, filename);

                // Force refresh if overwriting
                if (canOverwrite)
                {
                    AssetDatabase.ImportAsset(filename, ImportAssetOptions.ForceUpdate);
                }
            }

            // Import any new files
            foreach (var exportedPath in exportedPaths)
            {
                AssetDatabase.ImportAsset(exportedPath);
            }

            // Skip deletion when overwriting, to preserve the GUID
            if (AudioClipExporterEditor.ConfirmationWindow.deleteOriginalClips && !canOverwrite)
            {
                foreach (var asset in allAudioAssets.Select(a => a.clip).Distinct())
                {
                    Debug.Log("Deleting " + asset.name);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
                }
            }

            // Optionally clean up session
            if (AudioClipExporterEditor.ConfirmationWindow.deleteTimelineSession)
            {
                Undo.DestroyObjectImmediate(playableDirector.gameObject);
                RuntimeEditorHelper.EditorApplicationDelayCall(() =>
                    Selection.activeObject = GameObject.FindAnyObjectByType<Transform>());
            }

            // Select new/overwritten clip(s) in Project window
            Selection.objects = exportedPaths
                .ConvertAll(path => AssetDatabase.LoadAssetAtPath<AudioClip>(path))
                .Where(c => c != null)
                .ToArray();

            Debug.Log($"Export completed. {allAudioAssets.Count} clip(s) processed.");
        }

        public static string GetNewPath(string outputPath, string originalClipName, int index)
        {
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string letter = index < letters.Length ? letters[index].ToString() : index.ToString();
            return Path.Combine(outputPath, $"{originalClipName}_{letter}.wav");
        }

        private static AudioClip TrimAudioClip(AudioClip clip, float startTime, float endTime)
        {
            int frequency = clip.frequency;
            int channels = clip.channels;
            int startSample = Mathf.FloorToInt(startTime * frequency * channels);
            int endSample = Mathf.FloorToInt(endTime * frequency * channels);

            float[] originalData = new float[clip.samples * channels];
            clip.GetData(originalData, 0);

            int trimmedLength = endSample - startSample;
            float[] trimmedData = new float[trimmedLength];
            Array.Copy(originalData, startSample, trimmedData, 0, trimmedLength);

            AudioClip trimmedClip = AudioClip.Create(
                clip.name + "_trimmed", trimmedLength / channels, channels, frequency, false);
            trimmedClip.SetData(trimmedData, 0);
            return trimmedClip;
        }

        private static void SaveWavFile(AudioClip clip, string filename)
        {
            if (clip == null) return;

            int totalSamples = clip.samples * clip.channels;
            float[] samples = new float[totalSamples];
            if (!clip.GetData(samples, 0)) return;

            byte[] wavFile = ConvertToWav(samples, clip.channels, clip.frequency);
            if (wavFile == null || wavFile.Length == 0) return;

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

        private static byte[] ConvertToWav(float[] samples, int channels, int frequency)
        {
            int bitsPerSample = 16;
            int bytesPerSample = bitsPerSample / 8;
            int byteRate = frequency * channels * bytesPerSample;
            int blockAlign = channels * bytesPerSample;
            int dataSize = samples.Length * bytesPerSample;
            int fileSize = 44 + dataSize;

            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(new[] { 'R', 'I', 'F', 'F' });
                writer.Write(fileSize - 8);
                writer.Write(new[] { 'W', 'A', 'V', 'E' });

                writer.Write(new[] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(frequency);
                writer.Write(byteRate);
                writer.Write((short)blockAlign);
                writer.Write((short)bitsPerSample);

                writer.Write(new[] { 'd', 'a', 't', 'a' });
                writer.Write(dataSize);

                foreach (var sample in samples)
                {
                    short pcmSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                    writer.Write(pcmSample);
                }
                return memoryStream.ToArray();
            }
        }
    }
}

