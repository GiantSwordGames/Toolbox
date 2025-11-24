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
    [MenuItem("CONTEXT/Transform/Freeze/Local Y Position")]
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

        [MenuItem("CONTEXT/Transform/Freeze/Local X Position")]
        private static void SetXToZero(MenuCommand command)
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

            parent.localPosition = parent.localPosition.WithX(0);

            Vector3 delta = parent.position - originalPosition;
            foreach (Transform child in children)
            {
                RuntimeEditorHelper.RecordObjectUndo(child);
                child.position -= delta;
            }
        }


        [MenuItem("CONTEXT/Transform/Naming/Rename Prefab To Match Instance Name")]
        private static void RenamePrefabToMatchInstanceName(MenuCommand command)
        {
            RuntimeEditorHelper.RenamePrefabToMatchGameObject(((Transform)command.context).gameObject);
        }
        
        [MenuItem("CONTEXT/Transform/Naming/Rename Match Prefab Name")]
        private static void RenameToMatchPrefabName (MenuCommand command)
        {
            RuntimeEditorHelper.RenameToMatchPrefab(((Transform)command.context).gameObject);
        }

        [MenuItem("CONTEXT/Transform/Naming/Strip Duplicate Suffix")]
        private static void StripDuplicateSuffix(MenuCommand command)
        {
            RuntimeEditorHelper.StripDuplicateNumberFromName(((Transform)command.context).gameObject);
        }
         public static class TrimTextMeshProRect
    {
        [MenuItem("CONTEXT/TextMeshProUGUI/Trim Rect To Text Content")]
        private static void TrimRect(MenuCommand command)
        {
            var text = command.context as TextMeshProUGUI;
            if (text == null) return;
            var rect = text.rectTransform;
            Undo.RecordObject(rect, "Trim Rect To Text Content");

            // Force layout update and get preferred size
            text.ForceMeshUpdate();
            Vector2 preferred = text.GetPreferredValues(float.PositiveInfinity, float.PositiveInfinity);

            // Determine alignment offset within rect (0 = left/bottom, 0.5 = center, 1 = right/top)
            var align = text.alignment;
            float hAlign = 0.5f;
            float vAlign = 0.5f;

            switch (align)
            {
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.MidlineLeft:
                case TextAlignmentOptions.CaplineLeft:
                    hAlign = 0f; break;

                case TextAlignmentOptions.TopRight:
                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BaselineRight:
                case TextAlignmentOptions.MidlineRight:
                case TextAlignmentOptions.CaplineRight:
                    hAlign = 1f; break;
            }

            switch (align)
            {
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.Top:
                case TextAlignmentOptions.TopRight:
                    vAlign = 1f; break;

                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.Baseline:
                case TextAlignmentOptions.BaselineRight:
                    vAlign = 0f; break;
            }

            // Store world position of the rendered text origin (based on alignment)
            Vector2 oldSize = rect.rect.size;
            Vector2 oldOffset = new Vector2((hAlign - rect.pivot.x) * oldSize.x,
                                            (vAlign - rect.pivot.y) * oldSize.y);
            Vector3 oldWorldOrigin = rect.TransformPoint(oldOffset);

            // Resize rect
            rect.sizeDelta += preferred - oldSize;

            // Compute new world position of the same alignment point
            Vector2 newSize = rect.rect.size;
            Vector2 newOffset = new Vector2((hAlign - rect.pivot.x) * newSize.x,
                                            (vAlign - rect.pivot.y) * newSize.y);
            Vector3 newWorldOrigin = rect.TransformPoint(newOffset);

            // Offset to keep that alignment point fixed on screen
            Vector3 worldDelta = oldWorldOrigin - newWorldOrigin;
            rect.position += worldDelta;

            EditorUtility.SetDirty(rect);
        }

        [MenuItem("CONTEXT/TextMeshProUGUI/Trim Rect To Text Content", true)]
        private static bool ValidateTrimRect(MenuCommand command)
        {
            return command.context is TextMeshProUGUI;
        }
    }
        
        [MenuItem("CONTEXT/Transform/Freeze/Local Z Position")]
        private static void SetZToZero(MenuCommand command)
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

            parent.localPosition = parent.localPosition.WithZ(0);

            Vector3 delta = parent.position - originalPosition;
            foreach (Transform child in children)
            {
                RuntimeEditorHelper.RecordObjectUndo(child);
                child.position -= delta;
            }
        }

        [MenuItem("CONTEXT/Transform/Freeze/Position")]
        private static void ZeroPosition(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            RuntimeEditorHelper.ZeroPositionWithoutMovingChildren(parent);
        }


        [MenuItem("CONTEXT/Transform/Parenting/Move to Top of Siblings")]
        private static void MoveToTopOfSiblings(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            RuntimeEditorHelper.RecordObjectUndo(parent);
            parent.SetSiblingIndex(0);
            RuntimeEditorHelper.Focus(parent);
        }

      
        [MenuItem("CONTEXT/Transform/Position/Center On Children")]
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
        }
        
        [MenuItem("CONTEXT/Transform/Position/Round")]
        private static void Round(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
        
            Vector3 newPosition = parent.localPosition.Round();
            Vector3 oldPosition = parent.position;
            parent.localPosition = newPosition;
       
            RuntimeEditorHelper.RecordObjectUndo(parent);
        }

        [MenuItem("CONTEXT/Transform/Position/Align With First Child")]
        private static void AlignWithFirstChild(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
            if(children.Length ==0) 
                return;       

            Vector3 newPosition =children[0].position;
       
            RuntimeEditorHelper.RecordObjectUndo(parent);
            Vector3 delta = parent.position - newPosition;
            parent.position -= delta;
            foreach (Transform child in children)
            {
                RuntimeEditorHelper.RecordObjectUndo(child);
                child.position += delta;
            }
        }
        
        [MenuItem("CONTEXT/RectTransform/Encapsulate First Child")]
        private static void EncapsulateFirstChild(MenuCommand command)
        {
            RectTransform parent = (RectTransform)command.context;
            if (parent.childCount == 0)
            {
                Debug.LogWarning("Parent has no children to encapsulate.", parent);
                return;
            }

            RectTransform child = parent.GetChild(0) as RectTransform;
            if (child == null)
            {
                Debug.LogWarning("First child is not a RectTransform.", parent);
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Encapsulate First Child");

            // Get child's world corners
            Vector3[] worldCorners = new Vector3[4];
            child.GetWorldCorners(worldCorners);

            // Convert to parent local space
            Vector3 localMin = parent.InverseTransformPoint(worldCorners[0]);
            Vector3 localMax = localMin;
            for (int i = 1; i < 4; i++)
            {
                Vector3 localCorner = parent.InverseTransformPoint(worldCorners[i]);
                localMin = Vector3.Min(localMin, localCorner);
                localMax = Vector3.Max(localMax, localCorner);
            }

            // Desired size and center (in parent local space)
            Vector2 newSize = localMax - localMin;
            Vector2 newCenter = (localMax + localMin) * 0.5f;

            // Apply size and reposition relative to pivot
            parent.sizeDelta = newSize;
            parent.localPosition += (Vector3)(newCenter - Vector2.Scale(newSize, parent.pivot));

            Debug.Log($"Encapsulated {parent.name} tightly around {child.name}. Size = {newSize}", parent);
        }


        [MenuItem("CONTEXT/Transform/Freeze/Round Position")]
        private static void FreezeRound(MenuCommand command)
        {
            Transform parent = (Transform)command.context;
            Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
            if(children.Length ==0) 
                return;

            Vector3 newPosition = parent.position.Round();
       
            RuntimeEditorHelper.RecordObjectUndo(parent);
            Vector3 delta = parent.position - newPosition;
            parent.position -= delta;
            foreach (Transform child in children)
            {
                RuntimeEditorHelper.RecordObjectUndo(child);
                child.position += delta;
            }
        }

        
        [MenuItem("CONTEXT/Transform/Freeze/Scale")]
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
        
        [MenuItem("CONTEXT/Transform/Freeze/Transform")]
        private static void FreezeTransform(MenuCommand command)
        {
            NormalizeScale(command);
            NormalizeRotation(command);
            ZeroPosition(command);
        }

        [MenuItem("CONTEXT/Transform/Freeze/Rotation")]
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
        
        public static class RevertOverridesMenu
        {
            // The menu path shown when right-clicking a Transform or any component
            [MenuItem("CONTEXT/Component/Revert Overrides")]
            private static void RevertOverrides(MenuCommand command)
            {
                var component = command.context as Component;
                if (component == null)
                    return;

                if (PrefabUtility.IsPartOfPrefabInstance(component))
                {
                    Undo.RecordObject(component, "Revert Component Overrides");
                    PrefabUtility.RevertObjectOverride(component, InteractionMode.UserAction);
                    EditorUtility.SetDirty(component);
                    Debug.Log($"Reverted overrides on {component.GetType().Name} ({component.gameObject.name})");
                }
                else
                {
                    Debug.LogWarning($"'{component.name}' is not part of a prefab instance.");
                }
            }

            // Optional: You can limit visibility to prefab instances only
            [MenuItem("CONTEXT/Component/Revert Overrides", true)]
            private static bool Validate(MenuCommand command)
            {
                var component = command.context as Component;
                return component != null && PrefabUtility.IsPartOfPrefabInstance(component);
            }
        }
            
        
        [MenuItem("CONTEXT/Transform/Parenting/Reverse Child Order")]
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
            Renderer meshRenderer = command.context as Renderer;

            if (meshRenderer != null && meshRenderer.sharedMaterial != null)
            {
                // Duplicate the material
                Material originalMaterial = meshRenderer.sharedMaterial;
                Material duplicatedMaterial = new Material(originalMaterial);

                // Get the path of the original material
                string originalPath = AssetDatabase.GetAssetPath(originalMaterial);
                string directory = Path.GetDirectoryName(originalPath);

                string name = "M_" + meshRenderer.name + "2.mat";
                if (directory.Contains("com.unity") || directory == "Resources")
                {
                
                    directory =     MenuPaths.DEFAULT_PROJECT_PATH +"Materials";
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
                textMeshProUGUI.gameObject.name = newName.ToUpperCamelCase();
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
                textMeshProUGUI.gameObject.name = newName.ToUpperCamelCase();
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
           
        // [MenuItem("GameObject/Set As First Sibling", false, 18)]
        // public static void SetAsFirstSibling(MenuCommand command)
        // {
        //     Undo.RecordObject(Selection.activeGameObject.transform, "Set As First Sibling");
        //     Selection.activeGameObject.transform.SetSiblingIndex(0);
        // }
        //
        [MenuItem("CONTEXT/Transform/Parenting/Deparent")]
        public static void Deparent(MenuCommand command)
        {
            Transform transform = (Transform)command.context;
            Undo.SetTransformParent(transform, null, "Deparent");
        }

        [MenuItem("CONTEXT/Transform/Parenting/Move To Scene")]
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