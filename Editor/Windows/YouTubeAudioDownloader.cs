using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

namespace GiantSword
{
    public class YouTubeAudioDownloader : EditorWindow
    {
        private string _youtubeUrl = "";
        private string _startTime = "00:00:00";
        private string _endTime = "00:01:00";
        private string _outputFolder = "Assets/Project/Audio/Clips";

        [MenuItem("Windows/GiantSword/Download YouTube Audio")]
        public static void ShowWindow()
        {
            GetWindow<YouTubeAudioDownloader>("YouTube Audio Downloader");
        }

        void OnGUI()
        {
            GUILayout.Label("YouTube Audio Downloader", EditorStyles.boldLabel);

            _youtubeUrl = EditorGUILayout.TextField("YouTube URL", _youtubeUrl);
            _startTime = EditorGUILayout.TextField("Start Time (HH:MM:SS)", _startTime);
            _endTime = EditorGUILayout.TextField("End Time (HH:MM:SS)", _endTime);
            _outputFolder = EditorGUILayout.TextField("Output Folder", _outputFolder);

            if (GUILayout.Button("Download Audio"))
            {
                DownloadAudio();
            }
        }

        void DownloadAudio()
        {
            if (string.IsNullOrEmpty(_youtubeUrl)) return;

            string tempDir = Path.Combine(Application.dataPath, "../Temp/YTAudio");
            Directory.CreateDirectory(tempDir);

            string fileName = "yt_audio.%(ext)s";
            string tempOutput = Path.Combine(tempDir, fileName);
            string finalOutputPath = Path.Combine(_outputFolder, "yt_audio.mp3");

            // Calculate duration for trimming
            var start = System.TimeSpan.Parse(_startTime);
            var end = System.TimeSpan.Parse(_endTime);
            var duration = end - start;

            var ffmpegPath = "/opt/homebrew/bin/ffmpeg";
            var arguments = $"-x --audio-format mp3 " +
                            $"--ffmpeg-location \"{ffmpegPath}\" " +
                            $"--postprocessor-args \"-ss {start:c} -t {duration:c}\" " +
                            $"--output \"{tempOutput}\" " +
                            $"\"{_youtubeUrl}\"";
         

            var process = new Process();
            process.StartInfo.FileName = "/opt/homebrew/bin/yt-dlp";
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);
            process.ErrorDataReceived += (s, e) => UnityEngine.Debug.LogError(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            // Move file to Assets
            string downloadedPath = Directory.GetFiles(tempDir, "*.mp3")[0];
            string unityPath = Path.Combine(_outputFolder, Path.GetFileName(downloadedPath));
            Directory.CreateDirectory(_outputFolder);
            File.Copy(downloadedPath, unityPath, true);

            AssetDatabase.Refresh();

            UnityEngine.Debug.Log("Audio download complete!");
        }
    }
}
