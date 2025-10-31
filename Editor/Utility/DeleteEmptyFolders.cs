using System.IO;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public static class DeleteEmptyFolders
    {
        [MenuItem("Tools/Cleanup/Delete Empty Folders")]
        public static void DeleteEmpty()
        {
            string assetsPath = Application.dataPath;
            int deletedCount = 0;
            DeleteEmptyIn(assetsPath, ref deletedCount);

            AssetDatabase.Refresh();
            Debug.Log($"Deleted {deletedCount} empty folder(s).");
        }

        private static void DeleteEmptyIn(string path, ref int deletedCount)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteEmptyIn(directory, ref deletedCount);

                // Check again after recursive delete
                if (IsDirectoryEmpty(directory))
                {
                    Directory.Delete(directory, false);
                    string relativePath = "Assets" + directory.Replace(Application.dataPath, "").Replace("\\", "/");
                    Debug.Log($"Deleted empty folder: {relativePath}");
                    deletedCount++;
                }
            }
        }

        private static bool IsDirectoryEmpty(string path)
        {
            return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }
    }
}
