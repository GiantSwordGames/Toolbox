using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GiantSword;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace GiantSword
{
    [CreateAssetMenu(fileName = "PackageUpdater", menuName = MenuPaths.CREATE_MENU + "/PackageUpdater")]
    public class PackageUpdater : ScriptableObject
    {
        
        [SerializeField] private string _packageName = "com.giantsword.toolbox";
        private Preference<string> _localFilePath = null;
        [SerializeField] private string _gitRepoUrl = "https://github.com/GiantSwordGames/Toolbox.git";
        private PackageStatus _packageStatus;
        private string _version;
        private AddRequest addRequest;
        private RemoveRequest removeRequest;
        private string targetPackagePath;
        private ListRequest listRequest;

        public enum PackageStatus
        {
            Local,
            Git,
            NotInstalled,
            Switching
        }

        public PackageStatus packageStatus => _packageStatus;

         public string packageName
         {
             get => _packageName;
             set => _packageName = value;
         }

         public string localPackagePath
         {
             get => "file:"+localFilePath;
         }

         public string gitRepoUrl
         {
             get => _gitRepoUrl;
             set => _gitRepoUrl = value;
         }
         

         public string localFilePath
         {
             get
             {
                 LazyInitilizeLocalFilePath();
                 return _localFilePath.value;
             }
             set
             {
                 LazyInitilizeLocalFilePath();
                 _localFilePath.value = value;
             }
         }

         public string version => _version;


         private void LazyInitilizeLocalFilePath()
         {
             if (_localFilePath == null)
             {
                 string defaultPath =
                     Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Repositories");
                 _localFilePath = new Preference<string>(Application.productName+"_"+ this.name+"_localFilePath", defaultPath);
             }
         }

         public void CheckPackageStatus()
         {
            
             listRequest = Client.List(true);
             EditorApplication.update += CheckPackageStatusProgress;
         }

         private void CheckPackageStatusProgress()
         {
             if (listRequest.IsCompleted)
             {
                 if (listRequest.Status == StatusCode.Success)
                 {
                     foreach (var package in listRequest.Result)
                     {
                         if (package.name == packageName)
                         {
                             _version = package.version;
                             if (package.source == PackageSource.Local)
                             {
                                 _packageStatus = PackageStatus.Local;
                             }
                             else if (package.source == PackageSource.Git)
                             {
                                 _packageStatus = PackageStatus.Git;
                             }
                             return;
                         }
                     }
                     _packageStatus = PackageStatus.NotInstalled;
                 }
                 else if (listRequest.Status >= StatusCode.Failure)
                 {
                     Debug.LogError(listRequest.Error.message);
                 }

                 EditorApplication.update -= CheckPackageStatusProgress;
             }
         }
         public void SwitchToGitPackage()
         {
             targetPackagePath = gitRepoUrl;
             _packageStatus = PackageStatus.Switching;
             SwitchToPackageToTargetPath();
         }

         public void SwitchToLocalPackage()
         {
             if (File.Exists(localFilePath))
             {
                 targetPackagePath = localPackagePath;
                 SwitchToPackageToTargetPath();
             }
             else
             {
                    Debug.LogError("Local package path does not exist: " + localFilePath);
             }
            
         }
        private void SwitchToPackageToTargetPath()
        {
            listRequest = Client.List(true); // List all packages including local ones
            EditorApplication.update += ListPackagesProgress;
        }

        private void ListPackagesProgress()
        {
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {
                    bool packageFound = false;
                    foreach (var package in listRequest.Result)
                    {
                        if (package.name == packageName)
                        {
                            packageFound = true;
                            removeRequest = Client.Remove(packageName);
                            EditorApplication.update += RemovePackageProgress;
                            break;
                        }
                    }

                    if (!packageFound)
                    {
                        AddPackage(targetPackagePath);
                    }
                }
                else if (listRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError(listRequest.Error.message);
                }

                EditorApplication.update -= ListPackagesProgress;
            }
        }

        private void RemovePackageProgress()
        {
            if (removeRequest.IsCompleted)
            {
                if (removeRequest.Status == StatusCode.Success)
                {
                    AddPackage(targetPackagePath);
                }
                else if (removeRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError(removeRequest.Error.message);
                }

                EditorApplication.update -= RemovePackageProgress;
            }
        }

        private void AddPackage(string packagePath)
        {
            addRequest = Client.Add(packagePath);
            EditorApplication.update += AddPackageProgress;
        }

        private void AddPackageProgress()
        {
            if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log("Package successfully added: " + addRequest.Result.packageId);
                }
                else if (addRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError(addRequest.Error.message);
                }

                EditorApplication.update -= AddPackageProgress;
                CheckPackageStatus(); // Refresh package status after adding
            }
        }


        [Serializable]
        private class PackageData
        {
            public string name;
            public string version;
            public string displayName;
            public string description;
            public string unity;
            public Author author;
        }

        [Serializable]
        private class Author
        {
            public string name;
            public string email;
            public string url;
        }

        public void IncrementVersion()
        {
            string packageJsonPath = Path.Combine(localFilePath, "package.json");
            if (File.Exists(packageJsonPath))
            {
                string json = File.ReadAllText(packageJsonPath);
                var packageData = JsonUtility.FromJson<PackageData>(json);

                Version version;
                if (Version.TryParse(packageData.version, out version))
                {
                    version = new Version(version.Major, version.Minor, version.Build + 1);
                    packageData.version = version.ToString();
                    _version = version.ToString();
                    json = JsonUtility.ToJson(packageData, true);
                    File.WriteAllText(packageJsonPath, json);
                    Debug.Log("Version incremented to: " + _version);

                    // Trigger reimport of the package
                    AssetDatabase.ImportAsset(packageJsonPath, ImportAssetOptions.ForceUpdate);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("Invalid version format.");
                }
            }
            else
            {
                Debug.LogError("package.json not found at " + packageJsonPath);
            }
        }

        public void PublishNewVersion()
        {
            string packageJsonPath = Path.Combine(localFilePath, "package.json");
            if (File.Exists(packageJsonPath))
            {
                // Execute git commands
                // RunGitCommand("add " + packageJsonPath);
                RunGitCommand("add -all" );
                RunGitCommand("commit -m \"Update package version to " + _version + "\"");
                RunGitCommand("push");
                Debug.Log("New version published to the Git repository.");
            }
            else
            {
                Debug.LogError("package.json not found at " + packageJsonPath);
            }
        }

        private void RunGitCommand(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("git")
            {
                WorkingDirectory = localFilePath,
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.OutputDataReceived += (sender, args) => Debug.Log(args.Data);
                process.ErrorDataReceived += (sender, args) => Debug.LogError(args.Data);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }

         private void RegisterToolbarButton()
         {
             UnityToolbarExtender.farRight.Remove(OnToolbarGUI);
             UnityToolbarExtender.farRight.Add(OnToolbarGUI);
         }
       
         private void OnToolbarGUI()
         {
             GUILayoutOption buttonWidth = GUILayout.Width(50);
             switch (this.packageStatus)
             {
                 case PackageStatus.Switching:
                 {
                     bool cached = GUI.enabled;
                     GUI.enabled = false;
                     GUILayout.Button( new GUIContent("...", "Switching..."), buttonWidth);
                     GUI.enabled = cached;
                     break;
                 }
                 case PackageStatus.Git:
                 {
                     if(GUILayout.Button( new GUIContent("G → L", "Switch to Local"), buttonWidth))
                     {
                         Debug.Log("Switch to Local");
                         SwitchToLocalPackage();
                     }
                     break;
                 }
                 case PackageStatus.Local:
                 {
                     GUI.color = Color.yellow;
                     if(GUILayout.Button( new GUIContent("L → G", "Switch to Git"), buttonWidth))
                     {
                         Debug.Log("Switch to Git");
                         SwitchToGitPackage();
                     }
                     GUI.color = Color.white;

                     break;
                 }
             }
         }
         
         public static class Initializer
         {
             [InitializeOnLoadMethod]
             private static void InitializeOnLoad()
             {
                 List<PackageUpdater> packageUpdaters = RuntimeEditorHelper.FindAssetsOfType<PackageUpdater>();
                 foreach (var updater in packageUpdaters)
                 {
                     updater.CheckPackageStatus();
                     updater.RegisterToolbarButton();   
                 }

             }
         } 

    }
    
}