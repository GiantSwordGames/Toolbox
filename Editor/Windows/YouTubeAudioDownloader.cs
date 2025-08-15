using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

namespace JamKit
{
    public class YouTubeAudioDownloader : EditorWindow
    {
        private string youtubeUrl = "";
        private bool useStartTime = false;
        private bool useEndTime = false;
        private string startTime = "00:00:00";
        private string endTime = "00:01:00";
        private string outputFolder = "Assets/Project/Audio/Clips";

        private const string ytDlpPath = "/opt/homebrew/bin/yt-dlp"; // Update if needed
        private const string ffmpegPath = "/opt/homebrew/bin/ffmpeg"; // Update if needed

        [MenuItem(MenuPaths.WINDOWS + "Download YouTube Audio")]
        public static void ShowWindow()
        {
            var window = GetWindow<YouTubeAudioDownloader>("YouTube Audio Downloader");
            window.minSize = new Vector2(800, 230); // Double-width window
        }

        void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(youtubeUrl))
            {
                string clipboard = EditorGUIUtility.systemCopyBuffer;
                if (clipboard.StartsWith("http"))
                {
                    youtubeUrl = clipboard;
                }
            }
        }

        void OnGUI()
        {
            GUILayout.Label("YouTube Audio Downloader", EditorStyles.boldLabel);

            // YouTube URL input with paste button
            EditorGUILayout.BeginHorizontal();
            youtubeUrl = EditorGUILayout.TextField("YouTube URL", youtubeUrl);
            if (GUILayout.Button("Paste from Clipboard", GUILayout.Width(160)))
            {
                string clipboard = EditorGUIUtility.systemCopyBuffer;
                if (clipboard.StartsWith("http"))
                {
                    youtubeUrl = clipboard;
                    GUI.FocusControl(null); // remove focus from button
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            useStartTime = EditorGUILayout.Toggle("Use Start Time", useStartTime);
            startTime = EditorGUILayout.TextField("Start Time (HH:MM:SS)", startTime);

            useEndTime = EditorGUILayout.Toggle("Use End Time", useEndTime);
            endTime = EditorGUILayout.TextField("End Time (HH:MM:SS)", endTime);

            EditorGUILayout.Space();
            outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

            if (GUILayout.Button("Download Audio"))
            {
                DownloadAudio();
            }
        }

        void DownloadAudio()
        {
            if (string.IsNullOrWhiteSpace(youtubeUrl))
            {
                EditorUtility.DisplayDialog("Missing URL", "Please enter a valid YouTube URL.", "OK");
                return;
            }

            string tempDir = Path.Combine(Application.dataPath, "../Temp/YTAudio");
            Directory.CreateDirectory(tempDir);

            // Clean old mp3 files to avoid reusing previous downloads
            foreach (var file in Directory.GetFiles(tempDir, "*.mp3"))
            {
                try { File.Delete(file); } catch { }
            }

            string filePattern = "%(title)s.%(ext)s";
            string tempOutput = Path.Combine(tempDir, filePattern);

            // Optional trimming logic
            string postprocessorArgs = "";
            if (useStartTime && useEndTime)
            {
                var start = System.TimeSpan.Parse(startTime);
                var end = System.TimeSpan.Parse(endTime);
                var duration = end - start;
                postprocessorArgs = $"--postprocessor-args \"-ss {start:c} -t {duration:c}\"";
            }
            else if (useStartTime)
            {
                var start = System.TimeSpan.Parse(startTime);
                postprocessorArgs = $"--postprocessor-args \"-ss {start:c}\"";
            }
            else if (useEndTime)
            {
                var end = System.TimeSpan.Parse(endTime);
                postprocessorArgs = $"--postprocessor-args \"-t {end:c}\"";
            }

            string arguments =
                $"-x --audio-format mp3 " +
                $"--ffmpeg-location \"{ffmpegPath}\" " +
                $"--no-cache-dir --force-overwrites " +
                $"{postprocessorArgs} " +
                $"--output \"{tempOutput}\" " +
                $"\"{youtubeUrl}\"";

            var process = new Process();
            process.StartInfo.FileName = ytDlpPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.Log(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.LogError(e.Data);
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("yt-dlp execution failed: " + ex.Message);
                return;
            }

            string[] files = Directory.GetFiles(tempDir, "*.mp3");
            if (files.Length == 0)
            {
                UnityEngine.Debug.LogError("No MP3 file found in temp folder. Download may have failed.");
                return;
            }

            string downloadedPath = files[0];
            string filename = "Youtube_" + SanitizeFilename(Path.GetFileName(downloadedPath));
            string unityPath = Path.Combine(outputFolder, filename);

            Directory.CreateDirectory(outputFolder);
            File.Copy(downloadedPath, unityPath, true);

            AssetDatabase.Refresh();

            Object audioAsset = AssetDatabase.LoadAssetAtPath<Object>(unityPath);
            if (audioAsset != null)
            {
                Selection.activeObject = audioAsset;
                EditorGUIUtility.PingObject(audioAsset);
            }

            UnityEngine.Debug.Log("Audio download complete!");
        }

        string SanitizeFilename(string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                input = input.Replace(c.ToString(), "_");
            return input.Replace(" ", "_");
        }
    }
}
