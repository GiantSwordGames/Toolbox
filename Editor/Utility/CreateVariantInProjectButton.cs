using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public static class CreateVariantInProjectButton
    {
        
                [MenuItem(MenuPaths.QUICK_CREATE +"Create Duplicate In Project", false, MenuPaths.QUICK_CREATE_PRIORITY)]
               public static void CreateVariantInProject()
               {
                   // 1. Get the selected prefab
                   GameObject selectedPrefab = Selection.activeGameObject;
                   if (selectedPrefab == null)
                   {
                       Debug.LogError("No prefab selected.");
                       return;
                   }
       
                   // 2. Get the path to the selected prefab
                   string selectedPrefabPath = AssetDatabase.GetAssetPath(selectedPrefab);
       
                   // 3. Get the name of the selected prefab
                   string selectedPrefabName = selectedPrefab.name;
       
                   // 4. Get the path to the project folder
                   string projectFolderPath = "Assets/Project";
       
                   // 5. Get the path to the project prefabs folder
                   string projectPrefabsFolderPath = System.IO.Path.Combine(projectFolderPath, "Prefabs");
       
                   // 6. If the project prefabs folder does not exist, create it
                   if (!AssetDatabase.IsValidFolder(projectPrefabsFolderPath))
                   {
                       AssetDatabase.CreateFolder(projectFolderPath, "Prefabs");
                   }
       
                   // 7. Create a new variant of the selected prefab in the project prefabs folder
                   string newVariantPath = System.IO.Path.Combine(projectPrefabsFolderPath, selectedPrefabName + "_Variant.prefab");
                   PrefabUtility.SaveAsPrefabAsset(selectedPrefab, newVariantPath);
       
                   // 8. Log the path to the new variant
                   
                   // Select the created prefab
                     Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(newVariantPath);
                     Debug.Log("Variant created at: " + newVariantPath,Selection.activeObject);

               }
           }
       }
