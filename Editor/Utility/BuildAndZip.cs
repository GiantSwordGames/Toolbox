using System.IO;
using System.IO.Compression;
using System.Linq;
using GiantSword;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GiantSword
{
    public class BuildAndZip
    {
        public static Preference<bool> buildForMac = new Preference<bool>("BuildForMac", true);
        public static Preference<bool> buildForWindows = new Preference<bool>("BuildForWindows", true);
        public static Preference<bool> buildForLinux = new Preference<bool>("BuildForLinux", false);
        public static Preference<bool> buildForWebGL = new Preference<bool>("BuildForWeb", false);

        [MenuItem("Build/Build All Platforms")]
        public static void BuildAllPlatforms()
        {
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            string applicationName = Application.productName;
            string formattedAppName = applicationName.ToUpperCamelCase();
            string[] scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();

            if (buildForWindows.value)
            {
                string winBuildFolder = Path.Combine(desktopPath, formattedAppName + "_Win");
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
                ZipFile.CreateFromDirectory(winBuildFolder, zipPath);
                Debug.Log("Windows build zipped to: " + zipPath);
            }

            if (buildForMac.value)
            {
                string macBuildFolder = Path.Combine(desktopPath, formattedAppName + "_Mac");
                Directory.CreateDirectory(macBuildFolder);
                string macBuildPath = Path.Combine(macBuildFolder, applicationName + ".app");

                var macOptions = new BuildPlayerOptions
                {
                    scenes = scenePaths,
                    locationPathName = macBuildPath,
                    target = BuildTarget.StandaloneOSX,
                    options = BuildOptions.None
                };

                var report = BuildPipeline.BuildPlayer(macOptions);
                Debug.Log("Mac build completed: " + report.summary.result);

                DeleteDoNotShipArtifacts(macBuildFolder);

                string zipPath = macBuildFolder + ".zip";
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipFile.CreateFromDirectory(macBuildFolder, zipPath);
                Debug.Log("Mac build zipped to: " + zipPath);
            }

            if (buildForLinux.value)
            {
                string linuxBuildFolder = Path.Combine(desktopPath, formattedAppName + "_Linux");
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
                ZipFile.CreateFromDirectory(linuxBuildFolder, zipPath);
                Debug.Log("Linux build zipped to: " + zipPath);
            }

            if (buildForWebGL.value)
            {
                string webglBuildFolder = Path.Combine(desktopPath, formattedAppName + "_WebGL");
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
                ZipFile.CreateFromDirectory(webglBuildFolder, zipPath);
                Debug.Log("WebGL build zipped to: " + zipPath);
            }

            Debug.Log("All selected builds complete.");
        }

        private static void DeleteDoNotShipArtifacts(string buildRoot)
        {
            if (!Directory.Exists(buildRoot)) return;

            var directory = new DirectoryInfo(buildRoot);
            var matches = directory.GetFileSystemInfos("*DoNotShip*", SearchOption.AllDirectories);

            foreach (var file in matches)
            {
                try
                {
                    if (file is DirectoryInfo dir)
                    {
                        dir.Delete(true);
                        Debug.Log($"Deleted directory: {dir.FullName}");
                    }
                    else
                    {
                        file.Delete();
                        Debug.Log($"Deleted file: {file.FullName}");
                    }
                }
                catch (IOException e)
                {
                    Debug.LogWarning($"Failed to delete {file.FullName}: {e.Message}");
                }
            }
        }
    }
}
