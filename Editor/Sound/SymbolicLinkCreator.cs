using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GiantSword
{
    public static class SymbolicLinkCreator
    {
        private static readonly string AssetFolder = Application.dataPath + "/SoundCollections";

        public static void CreateSymbolicLink(string dropboxPath)
        {
            string projectPath = $"{AssetFolder}/{Path.GetFileName(dropboxPath)}";
            if (Directory.Exists(projectPath))
            {
                Debug.LogWarning("The symbolic link already exists.");
                return;
            }
            if (Directory.Exists(AssetFolder) == false)
            {
                Directory.CreateDirectory(projectPath);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "/bin/ln",
                Arguments = $"-s \"{dropboxPath}\" \"{projectPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Debug.Log(startInfo.Arguments);
            
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode == 0)
                {
                    Debug.Log("Symbolic link 'DropboxSoundAssets' created successfully.");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("Failed to create symbolic link: " + error);
                }
            }
        }
        
        public static class AssetRulePreferences
        {

            [InitializeOnLoadMethod]
            public static void InitializeSettings()
            {
                DeveloperPreferences.RegisterSettingDrawer(new DeveloperPreferences.SettingDrawer()
                {
                    keywords = new[] { "Dropbox Sound Library" },
                    onGUI = ()=>
                    {
                        DrawLinkButton("/Users/richard/Dropbox/SoundCollections/CuteFunCasualSounds");
                        DrawLinkButton("/Users/richard/Dropbox/SoundCollection/SeriousSounds");
                        DrawLinkButton("/Users/richard/Dropbox/SoundCollection/MassiveSoundCollections");
                    }
                });
            }

            private static void DrawLinkButton(string dropboxPath)
            {
                if (GUILayout.Button($"Create Symbolic Link To: {Path.GetFileName(dropboxPath)}", GUILayout.Width(400)))
                {
                    CreateSymbolicLink(dropboxPath);
                }
            }
        }
    }
}
