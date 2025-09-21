using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using JamKit;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace JamKit
{
    public class BuildAndZip
    {
        public static Preference<bool> buildForMac = new Preference<bool>("BuildForMac", true);
        public static Preference<bool> buildForWindows = new Preference<bool>("BuildForWindows", true);
        public static Preference<bool> buildForLinux = new Preference<bool>("BuildForLinux", false);
        public static Preference<bool> buildForWebGL = new Preference<bool>("BuildForWeb", false);
        public static Preference<string> specifiedBuildPath = new Preference<string>("BuildPath", "/Users/richard/Desktop/Builds");

        [InitializeOnLoadMethod]
        public static void InitializePreference()
        {
            DeveloperPreferences.RegisterPreference(specifiedBuildPath);
        }

        [MenuItem(MenuPaths.WINDOWS + "/Build All Platforms")]
        public static void BuildAllPlatforms()
        {
            string buildPath = specifiedBuildPath;
            string applicationName = Application.productName;
            string formattedAppName = applicationName.ToUpperCamelCase();

            // ✅ Collect only enabled & existing scenes
            string[] scenePaths = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .Where(path => !string.IsNullOrEmpty(path) && File.Exists(path))
                .ToArray();

            if (scenePaths.Length == 0)
            {
                Debug.LogError("No valid scenes found in Build Settings. Aborting build.");
                return;
            }

            string timestamp = DateTime.Now.ToString("yyMMdd_HHmm");

            if (buildForMac.value)
            {
                Debug.Log("Begin Mac Build:");

                string macBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_Mac");
                Directory.CreateDirectory(macBuildFolder);
                string macBuildPath = Path.Combine(macBuildFolder, applicationName + ".app");

                var macOptions = new BuildPlayerOptions
                {
                    scenes = scenePaths,
                    locationPathName = macBuildPath,
#if UNITY_2019_1
                    target = BuildTarget.StandaloneOSXUniversal,
#else
                    target = BuildTarget.StandaloneOSX,
#endif
                    options = BuildOptions.None
                };

                var report = BuildPipeline.BuildPlayer(macOptions);
                Debug.Log("Mac build completed: " + report.summary.result);

                DeleteDoNotShipArtifacts(macBuildFolder);

                string zipPath = macBuildFolder + ".zip";
                if (File.Exists(zipPath)) File.Delete(zipPath);

#if UNITY_EDITOR_OSX
                // Use macOS 'ditto' for preserving .app bundles properly
                string dittoArgs = $"-c -k --sequesterRsrc --keepParent \"{macBuildFolder}\" \"{zipPath}\"";
                var dittoProcess = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ditto",
                        Arguments = dittoArgs,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                dittoProcess.Start();
                dittoProcess.WaitForExit();
                Debug.Log("Mac build zipped using ditto: " + zipPath);
#else
                ZipUtil.CreateFromDirectory(macBuildFolder, zipPath);
                Debug.Log("Mac build zipped to: " + zipPath);
#endif

                Directory.Delete(macBuildFolder, true);
                Debug.Log("Deleted unzipped Mac build folder: " + macBuildFolder);
            }

            if (buildForWindows.value)
            {
                Debug.Log("Begin Win Build:");

                string winBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_Win");
                Directory.CreateDirectory(winBuildFolder);
                string winBuildPath = Path.Combine(winBuildFolder, applicationName + ".exe");

                var winOptions = new BuildPlayerOptions
                {
                    scenes = scenePaths,
                    locationPathName = winBuildPath,
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.None
                };

                var report = BuildPipeline.BuildPlayer(winOptions);
                Debug.Log("Windows build completed: " + report.summary.result);

                DeleteDoNotShipArtifacts(winBuildFolder);

                string zipPath = winBuildFolder + ".zip";
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipUtil.CreateFromDirectory(winBuildFolder, zipPath);
                Debug.Log("Windows build zipped to: " + zipPath);

                Directory.Delete(winBuildFolder, true);
                Debug.Log("Deleted unzipped Windows build folder: " + winBuildFolder);
            }

            if (buildForLinux.value)
            {
                Debug.Log("Begin Linux Build:");

                string linuxBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_Linux");
                Directory.CreateDirectory(linuxBuildFolder);
                string linuxBuildPath = Path.Combine(linuxBuildFolder, applicationName); // No extension

                var linuxOptions = new BuildPlayerOptions
                {
                    scenes = scenePaths,
                    locationPathName = linuxBuildPath,
                    target = BuildTarget.StandaloneLinux64,
                    options = BuildOptions.None
                };

                var report = BuildPipeline.BuildPlayer(linuxOptions);
                Debug.Log("Linux build completed: " + report.summary.result);

                DeleteDoNotShipArtifacts(linuxBuildFolder);

                string zipPath = linuxBuildFolder + ".zip";
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipUtil.CreateFromDirectory(linuxBuildFolder, zipPath);
                Debug.Log("Linux build zipped to: " + zipPath);

                Directory.Delete(linuxBuildFolder, true);
                Debug.Log("Deleted unzipped Linux build folder: " + linuxBuildFolder);
            }

            if (buildForWebGL.value)
            {
                Debug.Log("Begin Web Build:");

                string webglBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_WebGL");
                Directory.CreateDirectory(webglBuildFolder);

                var webglOptions = new BuildPlayerOptions
                {
                    scenes = scenePaths,
                    locationPathName = webglBuildFolder,
                    target = BuildTarget.WebGL,
                    options = BuildOptions.None
                };

                var report = BuildPipeline.BuildPlayer(webglOptions);
                Debug.Log("WebGL build completed: " + report.summary.result);

                DeleteDoNotShipArtifacts(webglBuildFolder);

                string zipPath = webglBuildFolder + ".zip";
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipUtil.CreateFromDirectory(webglBuildFolder, zipPath);
                Debug.Log("WebGL build zipped to: " + zipPath);

#if UNITY_EDITOR_OSX
                string escapedPath = webglBuildFolder.Replace(" ", "\\ ");
                string serverCommand = $"cd {escapedPath} && python3 -m http.server 8080";
                System.Diagnostics.Process.Start("open", $"-a Terminal \"{escapedPath}\"");
                System.Diagnostics.Process.Start("osascript", $"-e 'tell application \"Terminal\" to do script \"{serverCommand}\"'");
                System.Diagnostics.Process.Start("open", "http://localhost:8080");
#endif

                Directory.Delete(webglBuildFolder, true);
                Debug.Log("Deleted unzipped WebGL build folder: " + webglBuildFolder);
            }

            Debug.Log("All selected builds complete.");
#if UNITY_EDITOR_OSX
            System.Diagnostics.Process.Start("open", buildPath);
#elif UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", buildPath.Replace("/", "\\"));
#endif
        }

        private static void DeleteDoNotShipArtifacts(string buildRoot)
        {
            if (!Directory.Exists(buildRoot)) return;

            foreach (var path in Directory.EnumerateFileSystemEntries(buildRoot, "*DoNotShip*", SearchOption.AllDirectories))
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        Debug.Log($"Deleted directory: {path}");
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                        Debug.Log($"Deleted file: {path}");
                    }
                }
                catch (IOException e)
                {
                    Debug.LogWarning($"Failed to delete {path}: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Minimal ZipFile.CreateFromDirectory replacement that works on .NET Standard 2.0.
        /// </summary>
        private static class ZipUtil
        {
            public static void CreateFromDirectory(string sourceDirectory, string destinationZipPath)
            {
                var destDir = Path.GetDirectoryName(destinationZipPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                using (var fs = new FileStream(destinationZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    var basePath = Path.GetFullPath(sourceDirectory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

                    foreach (var filePath in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
                    {
                        if (string.Equals(Path.GetFullPath(filePath), Path.GetFullPath(destinationZipPath), StringComparison.OrdinalIgnoreCase))
                            continue;

                        var entryName = Path.GetFullPath(filePath).Substring(basePath.Length).Replace(Path.DirectorySeparatorChar, '/');
                        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

                        using (var entryStream = entry.Open())
                        using (var inputStream = File.OpenRead(filePath))
                        {
                            inputStream.CopyTo(entryStream);
                        }
                    }
                }
            }
        }
    }
}
