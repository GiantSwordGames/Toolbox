    using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JamKit
{
    #region Menu Items

    public static class GroupTool
    {

        [MenuItem( "Window/Tools/Group Selected %g")]
        private static void Group()
        {
            bool applyToProjectView = AssetDatabase.GetAssetPath(Selection.activeObject) != String.Empty;

            if (applyToProjectView)
            {
                GroupWindow window = GroupWindow.CreateWindow<GroupWindow>("Create Folder Group");
                window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 160);
                window.ShowModal();
            }
            else
            {
                GroupWindow.GroupGameObjects();
            }
        }
        
        [MenuItem( "Window/Tools/Group Alt &%g")]
        private static void GroupAlt()
        {
            GroupWindow.GroupGameObjects(true);

            // GroupWindow.UnGroupGameObjects();
        }
    }

    #endregion
    
    public class GroupWindow : EditorWindow
    {
        private static string _groupName = "Editor";
        private bool focused;
        
        private void Awake()
        {
            name = "Group";
            _groupName = Selection.activeObject.name + " group";
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Folder name:");
            GUI.SetNextControlName("name");
            _groupName = EditorGUILayout.TextField( _groupName);
       
            GUILayout.Space(20);
            
            if (GUILayout.Button("Create Group", GUILayout.Height(80)) || (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown))
            {
                CreateProjectFolder();
                GUIUtility.ExitGUI();
            }
            
            if (!focused)
            {
                EditorGUI.FocusTextInControl("name");
                focused = true;
            }
        }

        private void CreateProjectFolder()
        {
            Object[] files = Selection.objects;
            string originalPath = AssetDatabase.GetAssetPath(Selection.activeObject);
         
            string originalDirectory = Path.GetDirectoryName(originalPath);
            string newDirectory = Path.Combine( originalDirectory ,  _groupName);

            for (int i = 0; i < files.Length; i++)
            {
                originalPath = AssetDatabase.GetAssetPath(files[i]);
                string newPath = Path.Combine(newDirectory, Path.GetFileName(originalPath));

                if (Directory.Exists(newDirectory) == false)
                {
                    Directory.CreateDirectory(newDirectory);
                    AssetDatabase.ImportAsset(newDirectory);
                }
                AssetDatabase.MoveAsset(originalPath, newPath);
            }
            AssetDatabase.SaveAssets();
            Close();
        }

        public static void UnGroupGameObjects()
        {
            List<Object> newSelection = new List<Object>();
            List<Transform> parents = new List<Transform>();
            List<Transform> transforms = new List<Transform>( Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable));
            for (int i = 0; i < transforms.Count; i++)
            {
                Transform newParent = null;
                
                if(transforms[i].parent)
                    newParent = transforms[i].parent.parent;

                for (int j = transforms[i].childCount - 1; j >= 0; j--)
                {
                    Transform child = transforms[i].GetChild(j);
                    if (child.parent != null)
                    {
                        if (parents.Contains(child.parent) == false)
                            parents.Add(child.parent);
                    }
                    Undo.SetTransformParent(child, newParent,  "ungroup");
                    newSelection.Add(child.gameObject);
                }
            }

            for (int i = parents.Count - 1; i >= 0; i--)
            {
                // destroy parent if it is an empty gameobject
                if (parents[i].gameObject.GetComponents<Component>().Length <=1)
                {
                    Undo.DestroyObjectImmediate(parents[i].gameObject);
                }
                else
                {
                    newSelection.Add(parents[i].gameObject);
                }
            }

            Selection.objects = newSelection.ToArray();
        }
        public static void GroupGameObjects(bool alt = false)
        {
            List<Transform> transforms = new List<Transform>(Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable));
            if (transforms.Count > 0)
            {
                int index = transforms[0].GetSiblingIndex();

                GameObject group = new GameObject(transforms.Count > 1 ? "Group" : transforms[0].name);
                Undo.RegisterCreatedObjectUndo(group, "Group");
                EditorSceneManager.MoveGameObjectToScene(group, transforms[0].gameObject.scene);

                Transform commonParent = transforms[0].parent;
                Vector3 totalPosition = Vector3.zero;

                RectTransform childRectTransform = null;
                for (int i = 0; i < transforms.Count; i++)
                {
                    if (transforms[i].parent != commonParent)
                    {
                        commonParent = null;
                    }
                }
                
                for (int i = 0; i < transforms.Count; i++)
                {
                    if (transforms[i] is RectTransform)
                    {
                        childRectTransform = transforms[i] as RectTransform;   
                    }
                }

                if (childRectTransform)
                {
                    RectTransform rectTransform = group.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = childRectTransform.rect.size;
                    rectTransform.pivot = childRectTransform.pivot;
                    rectTransform.anchorMin = childRectTransform.anchorMin;
                    rectTransform.anchorMax = childRectTransform.anchorMax;
                    // rectTransform.anchoredPosition = childRectTransform.anchoredPosition;
                    // rectTransform.localRotation = childRectTransform.localRotation;
                    
                }

                Undo.SetTransformParent( group.transform, commonParent,  "Group");

                if (alt)
                {
                    group.transform.localPosition = transforms[0].localPosition;
                }
                else
                {
                    group.transform.localPosition = transforms[0].localPosition;
                    group.transform.localRotation = transforms[0].localRotation;
                    // group.transform.localScale = transforms[0].localScale;
                }

                for (int i = 0; i < transforms.Count; i++)
                    totalPosition += transforms[i].position;

                group.transform.position = totalPosition / transforms.Count;

                for (int i = 0; i < transforms.Count; i++)
                    Undo.SetTransformParent(transforms[i], group.transform, "Group");

                group.name = Selection.gameObjects[0].name;
                group.transform.SetSiblingIndex(index);
                Selection.activeGameObject = group;
            }
        }
    }
}