using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace JamKit
{
    public static class ContextExtensions
    {
        [MenuItem("CONTEXT/Transform/Set Y to Zero")]
        private static void SetYToZero(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
            children = System.Array.FindAll(children, t => t != parent);

            if (children.Length == 0)
            {
                Debug.LogWarning("No children found to center on.");
                return;
            }


            RuntimeEditorHelper.RecordObjectUndo(parent);

            Vector3 originalPosition = parent.position;

            parent.localPosition = parent.localPosition.WithY(0);

            Vector3 delta = parent.position - originalPosition;
            foreach (Transform child in children)
            {
                RuntimeEditorHelper.RecordObjectUndo(child);
                child.position -= delta;
            }
        }



        [MenuItem("CONTEXT/Transform/Zero Position")]
        private static void ZeroPosition(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
            children = System.Array.FindAll(children, t => t != parent);

            if (children.Length == 0)
            {
                Debug.LogWarning("No children found to center on.");
                return;
            }

           
            RuntimeEditorHelper.RecordObjectUndo(parent);
            
            Vector3 originalPosition = parent.position;

            parent.localPosition = Vector3.zero;
            
            Vector3 delta = parent.position - originalPosition;
            // parent.position -= delta;
            foreach (Transform child in children)
            {
                RuntimeEditorHelper.RecordObjectUndo(child);
                child.position -= delta;
            }

        }
        
        [MenuItem("CONTEXT/Transform/Center On Children")]
        private static void CenterOnChildren(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
            children = System.Array.FindAll(children, t => t != parent);

            if (children.Length == 0)
            {
                Debug.LogWarning("No children found to center on.");
                return;
            }

            Vector3 center = Vector3.zero;
            foreach (Transform child in children)
            {
                center += child.position;
            }
            center /= children.Length;

            RuntimeEditorHelper.RecordObjectUndo(parent);
            Vector3 delta = parent.position - center;
            parent.position -= delta;
            foreach (Transform child in children)
            {
                RuntimeEditorHelper.RecordObjectUndo(child);
                child.position += delta;
            }

            Debug.Log($"Centered '{parent.name}' on its children.");
        }
        [MenuItem("CONTEXT/Transform/Normalize/Normalize Scale")]
        private static void NormalizeScale(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
            children = System.Array.FindAll(children, t => t != parent);

            // Save world matrices of children
            Matrix4x4[] childWorldMatrices = new Matrix4x4[children.Length];
            for (int i = 0; i < children.Length; i++)
            {
                childWorldMatrices[i] = children[i].localToWorldMatrix;
                RuntimeEditorHelper.RecordObjectUndo(children[i]);
            }

            // Normalize parent's scale
            RuntimeEditorHelper.RecordObjectUndo(parent);
            parent.localScale = Vector3.one;

            // Re-apply each child's world matrix
            for (int i = 0; i < children.Length; i++)
            {
                Matrix4x4 worldMatrix = childWorldMatrices[i];
                Matrix4x4 parentWorldToLocal = parent.worldToLocalMatrix;
                Matrix4x4 newLocalMatrix = parentWorldToLocal * worldMatrix;

                // Decompose the new local matrix
                Vector3 pos = newLocalMatrix.GetColumn(3);
                Vector3 forward = newLocalMatrix.GetColumn(2);
                Vector3 upwards = newLocalMatrix.GetColumn(1);
                Vector3 scale = new Vector3(
                    newLocalMatrix.GetColumn(0).magnitude,
                    newLocalMatrix.GetColumn(1).magnitude,
                    newLocalMatrix.GetColumn(2).magnitude
                );
                Quaternion rot = Quaternion.LookRotation(forward, upwards);

                children[i].localPosition = pos;
                children[i].localRotation = rot;
                children[i].localScale = scale;
            }
        }

        [MenuItem("CONTEXT/Transform/Normalize/Normalize Rotation")]
        private static void NormalizeRotation(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
            children = System.Array.FindAll(children, t => t != parent);

            // Save world matrices of children
            Matrix4x4[] childWorldMatrices = new Matrix4x4[children.Length];
            for (int i = 0; i < children.Length; i++)
            {
                childWorldMatrices[i] = children[i].localToWorldMatrix;
                RuntimeEditorHelper.RecordObjectUndo(children[i]);
            }

            // Normalize parent's rotation
            RuntimeEditorHelper.RecordObjectUndo(parent);
            parent.localRotation = Quaternion.identity;

            // Re-apply each child's world matrix
            for (int i = 0; i < children.Length; i++)
            {
                Matrix4x4 worldMatrix = childWorldMatrices[i];
                Matrix4x4 parentWorldToLocal = parent.worldToLocalMatrix;
                Matrix4x4 newLocalMatrix = parentWorldToLocal * worldMatrix;

                // Decompose the new local matrix
                Vector3 pos = newLocalMatrix.GetColumn(3);
                Vector3 forward = newLocalMatrix.GetColumn(2);
                Vector3 upwards = newLocalMatrix.GetColumn(1);
                Vector3 scale = new Vector3(
                    newLocalMatrix.GetColumn(0).magnitude,
                    newLocalMatrix.GetColumn(1).magnitude,
                    newLocalMatrix.GetColumn(2).magnitude
                );
                Quaternion rot = Quaternion.LookRotation(forward, upwards);

                children[i].localPosition = pos;
                children[i].localRotation = rot;
                children[i].localScale = scale;
            }
        }
            
        
        [MenuItem("CONTEXT/Transform/Reverse Child Order")]
        private static void ReverseChildOrder(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            int childCount = parent.childCount;

            Undo.RegisterFullObjectHierarchyUndo(parent, "Reverse Child Order");

            // Detach and reattach children in reverse order
            Transform[] children = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                children[i] = parent.GetChild(i);
            }

            for (int i = childCount - 1; i >= 0; i--)
            {
                children[i].SetSiblingIndex(childCount - 1 - i);
            }

            Debug.Log($"Reversed child order of '{parent.name}'");
        }

        [MenuItem("CONTEXT/ParticleSystem/Assign Duplicate Material")]
        [MenuItem("CONTEXT/SkinnedMeshRenderer/Assign Duplicate Material")]
        [MenuItem("CONTEXT/MeshRenderer/Assign Duplicate Material")]
        [MenuItem("CONTEXT/TrailRenderer/Assign Duplicate Material")]
        [MenuItem("CONTEXT/LineRenderer/Assign Duplicate Material")]
        static void DuplicateAndAssignMaterial(MenuCommand command)
        {
            Renderer meshRenderer = (Renderer)command.context;

            if (meshRenderer != null && meshRenderer.sharedMaterial != null)
            {
                // Duplicate the material
                Material originalMaterial = meshRenderer.sharedMaterial;
                Material duplicatedMaterial = new Material(originalMaterial);

                // Get the path of the original material
                string originalPath = AssetDatabase.GetAssetPath(originalMaterial);
                string directory = Path.GetDirectoryName(originalPath);

                Debug.Log(directory);
                string name = "M_" + meshRenderer.name + "2.mat";
                if (directory.Contains("com.unity") || directory == "Resources")
                {
                    directory = "Assets/Project/Materials";
                }
                
                // create each directory if it does not exist 
                RuntimeEditorHelper.CreateFoldersIfNeeded(directory);


                string duplicatedPath = Path.Combine(directory, name);

                // Save the duplicated material as an asset in the same directory as the original material
                AssetDatabase.CreateAsset(duplicatedMaterial, duplicatedPath);
                AssetDatabase.SaveAssets();

                Undo.RecordObject(meshRenderer, "Duplicate");
                // Assign the duplicated material to the MeshRenderer
                meshRenderer.sharedMaterial = duplicatedMaterial;

                // Set the duplicated material as the active object in the selection
                // Selection.activeObject = duplicatedMaterial;
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
            AssetDatabase.CreateAsset(clip, directoryName + "/Anim_" + clip.name + ".anim"); // to create asset

            idleState.motion = clip;

            RuntimeEditorHelper.Ping(controller);
            (command.context as Animator).runtimeAnimatorController = controller;
        }


        [MenuItem("CONTEXT/MonoBehaviour/Name GameObject To Match Script")]
        public static void NameGameObjectToMatchScript(MenuCommand command)
        {
            MonoBehaviour monoBehaviour = (MonoBehaviour)command.context;
            if (monoBehaviour != null)
            {
                RuntimeEditorHelper.RecordObjectUndo(monoBehaviour.gameObject);
                string newName = monoBehaviour.GetType().Name;
                monoBehaviour.gameObject.name = newName;
            }
        }

        [MenuItem("GameObject/Rename Instance to Prefab Name", false, -9)]
        private static void RenameInstanceToPrefabName()
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                if (ValidationUtility.IsPrefabAsset(gameObject) == false)
                {
                    // Get the prefab asset
                    GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                    if (prefabAsset != null)
                    {
                        // Get the name of the prefab asset
                        string prefabName = prefabAsset.name;

                        // Record undo for the game object
                        RuntimeEditorHelper.RecordObjectUndo(gameObject);

                        // Rename the game object to match the prefab name
                        gameObject.name = prefabName;
                    }
                    
                }
            }
            
        }
        [MenuItem("CONTEXT/TextMeshProUGUI/Name GameObject To Match Text")]
        public static void NameGameObjectToMatchUGUIText(MenuCommand command)
        {
            TextMeshProUGUI textMeshProUGUI = (TextMeshProUGUI)command.context;
            if (textMeshProUGUI != null)
            {
                RuntimeEditorHelper.RecordObjectUndo(textMeshProUGUI.gameObject);
                string newName = textMeshProUGUI.text;
                textMeshProUGUI.gameObject.name = newName;
            }
        }
        
        [MenuItem("CONTEXT/TextMeshPro/Name GameObject To Match Text")]
        public static void NameGameObjectToMatchText(MenuCommand command)
        {
            TextMeshPro textMeshProUGUI = (TextMeshPro)command.context;
            if (textMeshProUGUI != null)
            {
                RuntimeEditorHelper.RecordObjectUndo(textMeshProUGUI.gameObject);
                string newName = textMeshProUGUI.text;
                textMeshProUGUI.gameObject.name = newName;
            }
        }

        [MenuItem("CONTEXT/SpriteRenderer/Rename Sprite To Match Game Object")]
        public static void RenameSpriteToMatchGameObject(MenuCommand command)
        {
            SpriteRenderer spriteRenderer = (SpriteRenderer)command.context;
            if (spriteRenderer != null)
            {
                Texture2D spriteTexture = spriteRenderer.sprite.texture;
                string path = AssetDatabase.GetAssetPath(spriteTexture);
                string directory = Path.GetDirectoryName(path);
                string newName = spriteRenderer.gameObject.name;
                string newPath = Path.Combine(directory, newName + ".png");
                AssetDatabase.MoveAsset(path, newPath);
                AssetDatabase.Refresh();
            }
        } 
        
        [MenuItem("CONTEXT/SpriteRenderer/Select Sprites")]
        public static void SelectSprites(MenuCommand command)
        {
            SpriteRenderer spriteRenderer = (SpriteRenderer)command.context;
            if (spriteRenderer != null)
            {
                RuntimeEditorHelper.AddToSelection( spriteRenderer.sprite.texture);
            }
        } 
           
        [MenuItem("GameObject/Set As First Sibling", false, 18)]
        public static void SetAsFirstSibling(MenuCommand command)
        {
            Undo.RecordObject(Selection.activeGameObject.transform, "Set As First Sibling");
            Selection.activeGameObject.transform.SetSiblingIndex(0);
        }
        
        [MenuItem("CONTEXT/Transform/Deparent")]
        public static void Deparent(MenuCommand command)
        {
            Transform transform = (Transform)command.context;
            Undo.SetTransformParent(transform, null, "Deparent");
        }

        [MenuItem("CONTEXT/Transform/Move To Scene")]
        public static void MoveToScene(MenuCommand command)
        {
            // create dropdown of open scenes
            EditorBuildSettingsScene[] scenePaths = EditorBuildSettings.scenes;
            List<string> sceneNames = new List<string>();
            foreach (var scenePath in scenePaths)
            {
                if (scenePath.enabled)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scenePath.path);
                    sceneNames.Add(sceneName);
                }
            }
            
            GenericMenu menu = new GenericMenu();
            foreach (var sceneName in sceneNames)
            {
                menu.AddItem(new GUIContent(sceneName), false, () =>
                {
                    string scenePath = sceneName + ".unity";
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    if (sceneAsset != null)
                    {
                        
                        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                        Transform transform = (Transform)command.context;
                        Undo.SetTransformParent(transform, null, "Move To Scene");
                        transform.position = Vector3.zero;
                    }
                });
            }
            
           
            
        }

        
    }
    
}