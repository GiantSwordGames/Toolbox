using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    [ExecuteInEditMode]
    public class ParentUnderSceneFolder : MonoBehaviour
    {
        [SerializeField] private SceneFolderAsset _sceneFolderAsset;
        [SerializeField] private bool _executeAtRuntime = false;

        private void Awake()
        {
            Apply();
        }

        private void Start()
        {
            
        }

        [Button]
        public void Apply()
        {
            try
            {


                if (gameObject.IsPrefabAsset())
                {
                    return;
                }

                if (enabled == false)
                    return;


                if (Application.isPlaying && _executeAtRuntime == false)
                {
                    return;
                }

                if (string.IsNullOrEmpty(gameObject.scene.path))
                {
                    return;
                }

                if (_sceneFolderAsset == null)
                {
                    return;
                }

                SceneFolder sceneFolder = SceneFolder.TryGetSceneFolder(_sceneFolderAsset);
                if (sceneFolder == null)
                {
                    Debug.Log($"Scene Folder ({_sceneFolderAsset}) not found for {name}", this);
                    return;
                }

                if (transform.parent != sceneFolder.transform)
                {
                    RuntimeEditorHelper.RecordObjectUndo(transform, "SceneFolder");
                    transform.SetParent(sceneFolder.transform, true);
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}