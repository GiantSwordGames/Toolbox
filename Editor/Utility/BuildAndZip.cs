using System.IO;
using System.IO.Compression;
using System.Linq;
using GiantSword;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GiantSword
{
    // Needed for LINQ methods

    public class BuildAndZip
    {
    
        public static class BuildWindowsAndMacToolBarButton
        {
            private static GUIContent _guiContent;

            [InitializeOnLoadMethod]
            private static void Initialize()
            {  
                UnityToolbarExtender.farRight.Add(OnToolbarGUI);
            }

            private static void OnToolbarGUI()
            {
                GUILayoutOption layoutWidth = GUILayout.Width( 26);
                GUILayoutOption layoutHeight = GUILayout.Height( 19);
                Texture texture = RuntimeEditorHelper.FindAssetByName<Texture>("Icon_BuildWinMac");
                _guiContent = new GUIContent(texture, "Build Windows And Mac");
                if(GUILayout.Button( _guiContent,layoutWidth, layoutHeight))
                {
                    BuildAllPlatforms();
                }
            }
        }
    
        [MenuItem("Build/Build Windows & Mac")]
        public static void BuildAllPlatforms()
        {
            // Get the desktop path
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            // Use your application name as defined in Player Settings
            string applicationName = Application.productName;

            // Define output paths
            string winBuildFolder = System.IO.Path.Combine(desktopPath, applicationName.ToUpperCamelCase() + "_Win");
            string macBuildFolder = System.IO.Path.Combine(desktopPath, applicationName.ToUpperCamelCase() + "_Mac");

            // Create directories if they don't exist
            if (!Directory.Exists(winBuildFolder))
                Directory.CreateDirectory(winBuildFolder);
            if (!Directory.Exists(macBuildFolder))
                Directory.CreateDirectory(macBuildFolder);

            // Convert EditorBuildSettings.scenes to a string array of scene paths
            string[] scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();

            // Build Windows version
            string winBuildPath = System.IO.Path.Combine(winBuildFolder, applicationName + ".exe");
            BuildPlayerOptions winOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = winBuildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            BuildReport winReport = BuildPipeline.BuildPlayer(winOptions);
            Debug.Log("Windows build completed: " + winReport.summary.result);

            // Zip the Windows build folder
            string winZipPath = winBuildFolder + ".zip";
            if (File.Exists(winZipPath))
                File.Delete(winZipPath);
            ZipFile.CreateFromDirectory(winBuildFolder, winZipPath);
            Debug.Log("Windows build zipped to: " + winZipPath);

            // Build Mac version
            // Note: For Mac builds, the output is a folder ending with .app.
            string macBuildPath = System.IO.Path.Combine(macBuildFolder, applicationName + ".app");
            BuildPlayerOptions macOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = macBuildPath,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.None
            };
            BuildReport macReport = BuildPipeline.BuildPlayer(macOptions);
            Debug.Log("Mac build completed: " + macReport.summary.result);

            // Zip the Mac build folder
            string macZipPath = macBuildFolder + ".zip";
            if (File.Exists(macZipPath))
                File.Delete(macZipPath);
            ZipFile.CreateFromDirectory(macBuildFolder, macZipPath);
            Debug.Log("Mac build zipped to: " + macZipPath);

            // Optional: Cleanup unzipped build directories after zipping
            // Directory.Delete(winBuildFolder, true);
            // Directory.Delete(macBuildFolder, true);

            Debug.Log("Build and zip process complete.");
        }
    }
}