using UnityEngine;
using UnityEditor;
using System.IO;

namespace GiantSword
{
    public static class PrefabVariantGenerator
    {
        // [MenuItem("Assets/Convert Prefab to Variant", false, 10)]
        public static void ConvertPrefabToVariant()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                Debug.LogError("No GameObject selected. Please select a prefab instance in the scene.");
                return;
            }

            string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Selected GameObject is not a prefab instance.");
                return;
            }

            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefabAsset == null)
            {
                Debug.LogError("Failed to load the original prefab asset.");
                return;
            }

            // Step 1: Read and store the GUID of the original prefab
            string originalGUID = ReadGUID(prefabAsset);
            if (originalGUID == null)
            {
                Debug.LogError("Failed to read the original prefab GUID.");
                return;
            }

            // Step 2: Assign a new GUID to the original prefab
            string newGUID = System.Guid.NewGuid().ToString().Replace("-","");
            WriteGUID(prefabAsset, newGUID);
            
            // Step 3: Reimport the original prefab to apply the new GUID
            AssetDatabase.ImportAsset(assetPath);
            Debug.Log($"Assigned new GUID to original prefab: {newGUID}");
             prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            // Step 4: Generate a variant of the original prefab
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
            if (tempInstance == null)
            {
                Debug.LogError("Failed to instantiate the prefab asset.");
                return;
            }

            string variantName = prefabAsset.name + "_Variant";
            string variantPath = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(assetPath) + "/" + variantName + ".prefab");

            GameObject prefabVariant = PrefabUtility.SaveAsPrefabAsset(tempInstance, variantPath);
            if (prefabVariant != null)
            {
                Debug.Log($"Prefab variant created at: {variantPath}");
            }
            else
            {
                Debug.LogError("Failed to create prefab variant.");
                Object.DestroyImmediate(tempInstance);
                return;
            }

            // Destroy the temporary instance
            Object.DestroyImmediate(tempInstance);

            // Step 5: Assign the original stored GUID to the variant
            WriteGUID(prefabVariant, originalGUID);
            AssetDatabase.ImportAsset(variantPath);
            Debug.Log($"Assigned original GUID to the variant: {originalGUID}");

            // Refresh the AssetDatabase to apply changes
            AssetDatabase.Refresh();
        }

        private static string ReadGUID(GameObject gameObject)
        {
            try
            {
                string assetPath = AssetDatabase.GetAssetPath(gameObject);
                string metaFilePath = assetPath + ".meta";
                string metaContent = File.ReadAllText(metaFilePath);
                const string guidPrefix = "guid: ";
                int startIndex = metaContent.IndexOf(guidPrefix) + guidPrefix.Length;
                if (startIndex < guidPrefix.Length) return null;

                int endIndex = metaContent.IndexOf("\n", startIndex);
                if (endIndex == -1) return null;

                return metaContent.Substring(startIndex, endIndex - startIndex).Trim();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("An error occurred while reading the GUID: " + ex.Message);
                return null;
            }
        }

        private static void WriteGUID(GameObject gameObject, string newGUID)
        {
            try
            {
                string assetPath = AssetDatabase.GetAssetPath(gameObject);
                string metaFilePath = assetPath + ".meta";
                string metaContent = File.ReadAllText(metaFilePath);
                const string guidPrefix = "guid: ";
                int startIndex = metaContent.IndexOf(guidPrefix) + guidPrefix.Length;
                if (startIndex < guidPrefix.Length)
                {
                    Debug.LogError("Failed to find the GUID in the .meta file.");
                    return;
                }

                int endIndex = metaContent.IndexOf("\n", startIndex);
                if (endIndex == -1)
                {
                    Debug.LogError("Failed to find the end of the GUID line in the .meta file.");
                    return;
                }

                // Replace the old GUID with the new one
                string oldGUID = metaContent.Substring(startIndex, endIndex - startIndex).Trim();
                metaContent = metaContent.Replace(oldGUID, newGUID);

                // Write the updated content back to the .meta file
                File.WriteAllText(metaFilePath, metaContent);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("An error occurred while writing the GUID: " + ex.Message);
            }
        }
    }
}
