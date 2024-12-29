
using System;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace GiantSword
{
   

    /// <summary>
    /// Class that makes it possible to serialize a reference to a scene object. Note that it will be serialized as a string name in builds.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Whether or not this contains a valid reference to a scene.
        /// </summary>
        public bool IsValid
        {
            get
            {
    #if UNITY_EDITOR
    
    
                return _sceneAsset != null;
    #else
                return !string.IsNullOrEmpty(_sceneName);
    #endif
            }
        }
    
        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string SceneName { get { return _sceneName; } }
    
    #if UNITY_EDITOR
    
        public UnityEditor.SceneAsset EditorSceneAsset { get { return _sceneAsset; } }
        public string EditorAssetPath { get { return UnityEditor.AssetDatabase.GetAssetPath(_sceneAsset); } }
    
        [SerializeField]
        private UnityEditor.SceneAsset _sceneAsset;
    
    
        public void Set(UnityEditor.SceneAsset sceneAsset)
        {
            _sceneAsset = sceneAsset;
            _sceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(_sceneAsset));
        }
    #endif
    
        [FormerlySerializedAs("_name")] [SerializeField]
        private string _sceneName;

        public bool isOpen
        {
            get { return isLoaded; }
        }

        public bool isLoaded
        {
            get { return SceneManager.GetSceneByName(SceneName).isLoaded; }
        }

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="mode">The scene loading mode to use</param>
        public void Load(LoadSceneMode mode = LoadSceneMode.Single)
        {
            Assert.IsTrue(IsValid, "Invalid scene");
    
            SceneManager.LoadScene(_sceneName, mode);
        }
    
    
        /// <summary>
        /// Loads the scene asynchronously.
        /// </summary>
        /// <param name="mode">The scene loading mode to use</param>
        public AsyncOperation LoadAsync(LoadSceneMode mode = LoadSceneMode.Single)
        {
            Assert.IsTrue(IsValid, "Invalid scene");
    
            return SceneManager.LoadSceneAsync(_sceneName, mode);
        }
    
        /// <summary>
        /// Sets this secne to be active
        /// </summary>
        /// <returns>True if the reference is valid and the scene is loaded</returns>
        public bool SetAsActiveScene()
        {
            if (IsValid)
            {
                return SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
            }
            return false;
        }
    
        public void OnBeforeSerialize()
        {
    
    
    #if UNITY_EDITOR
            _sceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(_sceneAsset));
    #endif
        }
    
        public void OnAfterDeserialize()
        {
    
        }

        // Compare Scene name 
        public override bool Equals(object obj)
        {
            if (obj is SceneReference sceneReference)
            {
                return sceneReference.SceneName == SceneName;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return SceneName.GetHashCode();
        }

        public void Set(Scene scene)
        {
#if UNITY_EDITOR
                UnityEditor.SceneAsset sceneAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(scene.path);
            Set(sceneAsset);
            #else
            Debug.LogWarning("Scene Reference Set Can Only Be Invoked From Editor");
#endif
      
        }

#if UNITY_EDITOR
        public void Open( UnityEditor.SceneManagement.OpenSceneMode openSceneMode =  UnityEditor.SceneManagement.OpenSceneMode.Single)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorAssetPath, openSceneMode);  
        }
#endif    


        public AsyncOperation UnlaodAsync()
        {
            if (isLoaded)
            {
                Scene scene = SceneManager.GetSceneByName(SceneName);
                AsyncOperation sceneAsync = SceneManager.UnloadSceneAsync(scene);
                return sceneAsync;
            }

            return null;
        }

        public void OpenAdditive()
        {
#if UNITY_EDITOR
            Open(OpenSceneMode.Additive);
#endif
        }
    }
}
