using System;
using System.Diagnostics;
using System.IO;
using GiantSword;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RichardPieterse
{
    
    
    [CustomEditor(typeof(PackageUpdater))]
    public class PackageUpdaterEditor : CustomEditorBase<PackageUpdater>
    {
        private void OnEnable()
        {
            targetObject.CheckPackageStatus();
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = EnableGUI();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_packageName"));
            
            GUILayout.BeginHorizontal();

            GUI.enabled = false;
            EditorGUILayout.TextField("Local Path", targetObject.localFilePath);
            GUI.enabled = true;
            if (GUILayout.Button("...", GUILayout.Width(16)))
            {
                var packageJsonFilePath =
                    EditorUtility.OpenFilePanel("Local package.json", targetObject.localFilePath, "json");
                
                Debug.Log(packageJsonFilePath);
                targetObject.localFilePath = Path.GetDirectoryName(packageJsonFilePath);
            }
            GUI.enabled = EnableGUI();

            GUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_gitRepoUrl"));
            
            GUILayout.Space(10);
            GUILayout.Label("Status: " + targetObject.packageStatus);
            GUILayout.Label("Version: " + targetObject.version);
            GUILayout.Space(10);

            GUI.enabled = true;

            GUILayout.BeginHorizontal();

            GUI.enabled = targetObject.packageStatus == PackageUpdater.PackageStatus.Git;

            if (GUILayout.Button("Switch to Local"))
            {
                targetObject.SwitchToLocalPackage();
            }

            GUI.enabled = targetObject.packageStatus == PackageUpdater.PackageStatus.Local;
            
            if (GUILayout.Button("Switch to Git"))
            {
                targetObject.SwitchToGitPackage();
            }

            GUILayout.Space(10);


            if (GUILayout.Button("Refresh Status"))
            {
                targetObject.CheckPackageStatus();
            }
            
            GUILayout.Space(10);

            GUI.enabled = EnableGUI();
            if (GUILayout.Button("Increment Version"))
            {
                targetObject.IncrementVersion();
            }

            if (GUILayout.Button("Publish New Version"))
            {
                targetObject.PublishNewVersion();
            }

            GUILayout.EndHorizontal();
        }

        private bool EnableGUI()
        {
            return targetObject.packageStatus == PackageUpdater.PackageStatus.Local;
        }


    }
}
