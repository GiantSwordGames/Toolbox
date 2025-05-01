using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GiantSword
{
    
    [CreateAssetMenu(menuName = MenuPaths.CREATE_ASSET_MENU +"Level", fileName = "Level_NewLevel")]
    public class Level : ScriptableObject
    {
        [SerializeField]  private SceneReference _scene;
        [SerializeField]  private SceneReference[] _additionalScene;
        [SerializeField]  private SceneReference[] _persistentScene;

        public SceneReference scene => _scene;

        List<SceneReference> GetAllSceneReferences()
        {
            List<SceneReference> allScenes = new List<SceneReference>();
            allScenes.Add(_scene);
            allScenes.AddRange(_additionalScene);
            allScenes.AddRange(_persistentScene);
            return allScenes;
        }

        private void RuntimeLoadStep()
        {
            if (_scene.isLoaded == false)
            {
                if(SceneManager.sceneCount > 1)
                {
                    _scene.Load(LoadSceneMode.Additive);
                }
                else
                {
                    _scene.Load(LoadSceneMode.Single);
                }
            }
            
            foreach (var scene in _additionalScene)
            {
                if (scene.isLoaded == false)
                {
                    scene.Load(LoadSceneMode.Additive);
                }
            }
                
            foreach (var scene in _persistentScene)
            {
                if (scene.isLoaded == false)
                {
                    
                    scene.Load(LoadSceneMode.Additive);
                }
            }
        }


        public void OpenLevel()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                int count = 0;
                List<SceneReference> allScenes = GetAllSceneReferences();
                foreach (var scene in allScenes)
                {
                    if (count == 0)
                    {
                        scene.Open(OpenSceneMode.Single);
                    }
                    else
                    {
                        scene.Open(OpenSceneMode.Additive);
                    }

                    count++;
                }
            }
            #endif
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void LoadLevel()
        {
            AsyncHelper.StartCoroutine(IELoadLevel());
        }
        private IEnumerator IELoadLevel()
        {

            if (Application.isPlaying)
            {
                List<SceneReference> allScenes = GetAllSceneReferences();

                List<AsyncOperation> _unloadCommands = new List<AsyncOperation>();
                Debug.Log(" SceneManager.loadedSceneCount " + SceneManager.loadedSceneCount);

                //unload other scenes
                for (int i = SceneManager.loadedSceneCount - 1; i >= 0; i--)
                {

                    Scene sceneAt = SceneManager.GetSceneAt(i);
                    if (allScenes.Contains(sceneAt) == false)
                    {
                        // unload scene
                        if (i >= 1)
                        {
                            AsyncOperation unloadSceneAsync = SceneManager.UnloadSceneAsync(sceneAt);
                            _unloadCommands.Add(unloadSceneAsync);
                        }
                    }
                }

                while (true)
                {
                    bool complete = true;
                    foreach (var asyncOperation in _unloadCommands)
                    {
                        try
                        {
                            if(asyncOperation.isDone == false)
                            {
                                complete = false;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                      
                    }

                    if (complete)
                    {
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }

                if (scene.isLoaded)
                {
                    if (SceneManager.loadedSceneCount > 1)
                    {
                        AsyncOperation asyncOperation = _scene.UnlaodAsync();

                        asyncOperation.completed += operation =>
                        {
                            RuntimeLoadStep();
                        };
                    }
                    else
                    {
                        _scene.Load(LoadSceneMode.Single ); // we hard load this scene if it is the only one in the scene as
                        RuntimeLoadStep();
                    }

                }
                else
                {
                    RuntimeLoadStep();
                }
            }
            else
            {

#if UNITY_EDITOR
                
                _scene.Open();
                
                foreach (var scene in _persistentScene)
                {
                    if (scene.isOpen == false)
                    {
                        scene.Open(  UnityEditor.SceneManagement.OpenSceneMode.Additive);
                    }
                }
#endif

            }
            
            yield break;
            
        }

        public void ReloadLevel()
        {
            LoadLevel();
        }
    }
}