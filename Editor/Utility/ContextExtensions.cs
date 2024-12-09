using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace GiantSword
{
    public  static class ContextExtensions 
    {
       
        // Add the option to the context menu for the MeshRenderer component
      [MenuItem("CONTEXT/MeshRenderer/Assign Duplicate Material")]
static void DuplicateAndAssignMaterial(MenuCommand command)
{
    MeshRenderer meshRenderer = (MeshRenderer)command.context;

    if (meshRenderer != null && meshRenderer.sharedMaterial != null)
    {
        // Duplicate the material
        Material originalMaterial = meshRenderer.sharedMaterial;
        Material duplicatedMaterial = new Material(originalMaterial);

        // Get the path of the original material
        string originalPath = AssetDatabase.GetAssetPath(originalMaterial);
        string directory = Path.GetDirectoryName(originalPath);

        string name = "Material_" +meshRenderer.name + "_Duplicate.mat";
        if (directory.Contains("com.unity"))
        {
            directory = "Assets/Project/Materials";
        }
        string duplicatedPath = Path.Combine(directory, name);

        // Save the duplicated material as an asset in the same directory as the original material
        AssetDatabase.CreateAsset(duplicatedMaterial, duplicatedPath);
        AssetDatabase.SaveAssets();

        Undo.RecordObject(meshRenderer, "Duplicate");
        // Assign the duplicated material to the MeshRenderer
        meshRenderer.sharedMaterial = duplicatedMaterial;

        // Set the duplicated material as the active object in the selection
        Selection.activeObject = duplicatedMaterial;
    }
}
        
        [MenuItem("CONTEXT/Animator/Create Controller With Empty Idle")]
        public static void CreateTwoStateAnimator(MenuCommand command)
        {
            string directory = Application.dataPath + "/Silverlake/Animations";
            Debug.Log(directory);
            string filePath = EditorUtility.SaveFilePanel("Choose Location",
                directory, "Animator_TwoState_UniqueName", ".controller");
            // Debug.Log(filePath);
            
            if (filePath == "")
            {
                return;
            }
            string projectPath = "Assets/" + filePath.Replace(Application.dataPath, "") + ".asset";
    
            var controller = AnimatorController.CreateAnimatorControllerAtPath(projectPath);
            
            var idleState = controller.layers[0].stateMachine.AddState("Idle");
        
            var clip = new AnimationClip();
            clip.name = "Idle"; // set name


            string directoryName = Path.GetDirectoryName(projectPath);
            directoryName = directoryName.Replace(Application.dataPath, "Assets/");
            AssetDatabase.CreateAsset(clip, directoryName+ "/Anim_" +clip.name+".anim"); // to create asset
            
            idleState.motion = clip;

            RuntimeEditorHelper.Ping(controller);
            (command.context as Animator).runtimeAnimatorController = controller;
        }
    }
}