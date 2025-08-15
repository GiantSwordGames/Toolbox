using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public static class ImportAudioFromDownloads
    {
        public static void CopyAudioFilesFromDownloadsToAssets()
        {
            // Define the paths for the Downloads and Assets directories
            string downloadsPath =
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
                    "Downloads");

            string projectLocation = "Project/Audio";
            // string projectLocation = "Project/Audio/Clips/SoundEffects";
            string assetsPath = Path.Combine(Application.dataPath, projectLocation);

            // Define the audio file extensions to look for
            string[] audioExtensions = new string[] { "*.mp3", "*.wav", "*.flac" };

            List<Object> importedAssets = new List<Object>();

            // Copy each audio file to the Assets folder
            foreach (string extension in audioExtensions)
            {
                string[] audioFiles = Directory.GetFiles(downloadsPath, extension);
                foreach (string filePath in audioFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    string destinationPath = Path.Combine(assetsPath, fileName);

                    try
                    {
                        File.Copy(filePath, destinationPath, true);
                        File.Delete(filePath);
                        Debug.Log($"Copied and deleted {fileName} from Downloads folder.");

                        // Import the copied file into Unity
                        string relativePath = $"Assets/{projectLocation}/{fileName}";
                        AssetDatabase.ImportAsset(relativePath);
                        Debug.Log($"Imported {fileName} into Unity.");

                        // Add the imported asset to the list
                        Object importedAsset = AssetDatabase.LoadAssetAtPath<Object>(relativePath);
                        if (importedAsset != null)
                        {
                            importedAssets.Add(importedAsset);
                        }
                    }
                    catch (IOException ex)
                    {
                        Debug.LogError($"Failed to copy or delete {fileName}: {ex.Message}");
                    }
                }
            }

            // Select the imported assets in Unity
            if (importedAssets.Count > 0)
            {
                Selection.objects = importedAssets.ToArray();
                Debug.Log("Selected imported audio files in Unity.");
            }
        }
    }
}