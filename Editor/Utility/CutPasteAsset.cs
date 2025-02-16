using UnityEngine;
using UnityEditor;
using System.IO;

public class CutPasteAsset
{
    // Store the asset path when cut.
    private static string assetPathToCut = null;

    [MenuItem("Assets/Cut", false, 49)]
    public static void CutAsset()
    {
        // Get the asset path of the currently selected object.
        if (Selection.activeObject == null)
        {
            Debug.LogWarning("No asset selected to cut.");
            return;
        }

        assetPathToCut = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(assetPathToCut))
        {
            Debug.LogWarning("Selected object is not a valid asset.");
            return;
        }

        Debug.Log("Asset cut: " + assetPathToCut);
    }

    [MenuItem("Assets/Paste", false, 49)]
    public static void PasteAsset()
    {
        if (string.IsNullOrEmpty(assetPathToCut))
        {
            Debug.LogWarning("No asset has been cut.");
            return;
        }

        // Determine the target folder based on current selection.
        string targetFolder = GetTargetFolder();
        if (string.IsNullOrEmpty(targetFolder))
        {
            Debug.LogWarning("Please select a target folder in the Project view to paste the asset.");
            return;
        }

        // Get the file name and generate a unique asset path in the target folder.
        string fileName = Path.GetFileName(assetPathToCut);
        string targetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(targetFolder, fileName));

        // Move the asset.
        string error = AssetDatabase.MoveAsset(assetPathToCut, targetPath);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Error moving asset: " + error);
        }
        else
        {
            Debug.Log("Asset pasted: " + targetPath);
            // Clear the cut asset since the move succeeded.
            assetPathToCut = null;
        }
    }

    // Helper method to determine the target folder from the current selection.
    private static string GetTargetFolder()
    {
        string folderPath = "";

        // If a folder is selected, use its path.
        if (Selection.activeObject != null)
        {
            folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                // If a non-folder is selected, get its parent folder.
                folderPath = Path.GetDirectoryName(folderPath);
            }
        }

        return folderPath;
    }
}
