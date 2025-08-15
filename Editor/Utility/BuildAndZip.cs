using System;
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
        public static Preference<string> specifiedBuildPath = new Preference<string>("BuildPath", "/Users/richard/Desktop/Builds");

        [InitializeOnLoadMethod]
        public static void InitializePreference()
        {
            DeveloperPreferences.RegisterPreference(specifiedBuildPath);
        }
        
        [MenuItem( MenuPaths.WINDOWS +"/Build All Platforms")]
        public static void BuildAllPlatforms()
        {
        //     string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //     Debug.Log("specifiedBuildPath exists " + Directory.Exists(specifiedBuildPath));
        //     string buildPath = specifiedBuildPath;
        //     string applicationName = Application.productName;
        //     string formattedAppName = applicationName.ToUpperCamelCase();
        //     string[] scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
        //     string timestamp = DateTime.Now.ToString("yyMMdd_HHmm");
        //
        //     if (buildForMac.value)
        //     {
        //         Debug.Log("Begin Mac Build:");
        //
        //         string macBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_Mac");
        //         Directory.CreateDirectory(macBuildFolder);
        //         string macBuildPath = Path.Combine(macBuildFolder, applicationName + ".app");
        //
        //         var macOptions = new BuildPlayerOptions
        //         {
        //             scenes = scenePaths,
        //             locationPathName = macBuildPath,
        //             target = BuildTarget.StandaloneOSX,
        //             options = BuildOptions.None
        //         };
        //
        //         var report = BuildPipeline.BuildPlayer(macOptions);
        //         Debug.Log("Mac build completed: " + report.summary.result);
        //
        //         DeleteDoNotShipArtifacts(macBuildFolder);
        //
        //         string zipPath = macBuildFolder + ".zip";
        //         if (File.Exists(zipPath)) File.Delete(zipPath);
        //
        //         string dittoArgs = $"-c -k --sequesterRsrc --keepParent \"{macBuildFolder}\" \"{zipPath}\"";
        //         var dittoProcess = new System.Diagnostics.Process
        //         {
        //             StartInfo = new System.Diagnostics.ProcessStartInfo
        //             {
        //                 FileName = "ditto",
        //                 Arguments = dittoArgs,
        //                 RedirectStandardOutput = true,
        //                 UseShellExecute = false,
        //                 CreateNoWindow = true
        //             }
        //         };
        //         dittoProcess.Start();
        //         dittoProcess.WaitForExit();
        //
        //         Debug.Log("Mac build zipped using ditto: " + zipPath);
        //
        //         Directory.Delete(macBuildFolder, true);
        //         Debug.Log("Deleted unzipped Mac build folder: " + macBuildFolder);
        //     }
        //
        //     if (buildForWindows.value)
        //     {
        //         Debug.Log("Begin Win Build:");
        //
        //         string winBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_Win");
        //         Directory.CreateDirectory(winBuildFolder);
        //         string winBuildPath = Path.Combine(winBuildFolder, applicationName + ".exe");
        //
        //         var winOptions = new BuildPlayerOptions
        //         {
        //             scenes = scenePaths,
        //             locationPathName = winBuildPath,
        //             target = BuildTarget.StandaloneWindows64,
        //             options = BuildOptions.None
        //         };
        //
        //         var report = BuildPipeline.BuildPlayer(winOptions);
        //         Debug.Log("Windows build completed: " + report.summary.result);
        //
        //         DeleteDoNotShipArtifacts(winBuildFolder);
        //
        //         string zipPath = winBuildFolder + ".zip";
        //         if (File.Exists(zipPath)) File.Delete(zipPath);
        //         ZipFile.CreateFromDirectory(winBuildFolder, zipPath);
        //         Debug.Log("Windows build zipped to: " + zipPath);
        //
        //         Directory.Delete(winBuildFolder, true);
        //         Debug.Log("Deleted unzipped Windows build folder: " + winBuildFolder);
        //     }
        //
        //     if (buildForLinux.value)
        //     {
        //         Debug.Log("Begin Linux Build:");
        //
        //         string linuxBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_Linux");
        //         Directory.CreateDirectory(linuxBuildFolder);
        //         string linuxBuildPath = Path.Combine(linuxBuildFolder, applicationName); // No extension
        //
        //         var linuxOptions = new BuildPlayerOptions
        //         {
        //             scenes = scenePaths,
        //             locationPathName = linuxBuildPath,
        //             target = BuildTarget.StandaloneLinux64,
        //             options = BuildOptions.None
        //         };
        //
        //         var report = BuildPipeline.BuildPlayer(linuxOptions);
        //         Debug.Log("Linux build completed: " + report.summary.result);
        //
        //         DeleteDoNotShipArtifacts(linuxBuildFolder);
        //
        //         string zipPath = linuxBuildFolder + ".zip";
        //         if (File.Exists(zipPath)) File.Delete(zipPath);
        //         ZipFile.CreateFromDirectory(linuxBuildFolder, zipPath);
        //         Debug.Log("Linux build zipped to: " + zipPath);
        //
        //         Directory.Delete(linuxBuildFolder, true);
        //         Debug.Log("Deleted unzipped Linux build folder: " + linuxBuildFolder);
        //     }
        //
        //     if (buildForWebGL.value)
        //     {
        //         Debug.Log("Begin Web Build:");
        //
        //         string webglBuildFolder = Path.Combine(buildPath, $"{formattedAppName}_{timestamp}_WebGL");
        //         Directory.CreateDirectory(webglBuildFolder);
        //
        //         var webglOptions = new BuildPlayerOptions
        //         {
        //             scenes = scenePaths,
        //             locationPathName = webglBuildFolder,
        //             target = BuildTarget.WebGL,
        //             options = BuildOptions.None
        //         };
        //
        //         var report = BuildPipeline.BuildPlayer(webglOptions);
        //         Debug.Log("WebGL build completed: " + report.summary.result);
        //
        //         DeleteDoNotShipArtifacts(webglBuildFolder);
        //
        //         string zipPath = webglBuildFolder + ".zip";
        //         if (File.Exists(zipPath)) File.Delete(zipPath);
        //         ZipFile.CreateFromDirectory(webglBuildFolder, zipPath);
        //         Debug.Log("WebGL build zipped to: " + zipPath);
        //
        //         string escapedPath = webglBuildFolder.Replace(" ", "\\ ");
        //         string serverCommand = $"cd {escapedPath} && python3 -m http.server 8080";
        //         System.Diagnostics.Process.Start("open", $"-a Terminal \"{escapedPath}\"");
        //         System.Diagnostics.Process.Start("osascript", $"-e 'tell application \"Terminal\" to do script \"{serverCommand}\"'");
        //         System.Diagnostics.Process.Start("open", "http://localhost:8080");
        //
        //         Directory.Delete(webglBuildFolder, true);
        //         Debug.Log("Deleted unzipped WebGL build folder: " + webglBuildFolder);
        //     }
        //
        //     Debug.Log("All selected builds complete.");
        //     System.Diagnostics.Process.Start(buildPath);
        // }
        //
        // private static void DeleteDoNotShipArtifacts(string buildRoot)
        // {
        //     if (!Directory.Exists(buildRoot)) return;
        //
        //     var directory = new DirectoryInfo(buildRoot);
        //     var matches = directory.GetFileSystemInfos("*DoNotShip*", SearchOption.AllDirectories);
        //
        //     foreach (var file in matches)
        //     {
        //         try
        //         {
        //             if (file is DirectoryInfo dir)
        //             {
        //                 dir.Delete(true);
        //                 Debug.Log($"Deleted directory: {dir.FullName}");
        //             }
        //             else
        //             {
        //                 file.Delete();
        //                 Debug.Log($"Deleted file: {file.FullName}");
        //             }
        //         }
        //         catch (IOException e)
        //         {
        //             Debug.LogWarning($"Failed to delete {file.FullName}: {e.Message}");
        //         }
        //     }
        }
    }
}
