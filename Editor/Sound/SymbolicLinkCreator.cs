using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GiantSword
{
    public static class SymbolicLinkCreator
    {
        private static readonly string DropboxPath = "/Users/richard/Dropbox/SoundCollection";
        private static readonly string LinkPath = Application.dataPath + "/SoundCollection";

        public static void CreateSymbolicLink()
        {
            if (Directory.Exists(LinkPath))
            {
                Debug.LogWarning("The symbolic link already exists.");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "/bin/ln",
                Arguments = $"-s \"{DropboxPath}\" \"{LinkPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

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
                        if (GUILayout.Button("Create Symbolic Link To Dropbox Sound Library", GUILayout.Width(400)))
                        {
                            CreateSymbolicLink();
                        }
                        // _checkRulesOnAssetImport.value = GUILayout.Toggle(_checkRulesOnAssetImport.value, "Check Naming Conventions On Asset Import");
                        // autoApplyNamingConventions.value = GUILayout.Toggle(autoApplyNamingConventions.value, "Auto Apply Naming Conventions On Asset Import");
                    }
                });
            }
        }
    }
}
