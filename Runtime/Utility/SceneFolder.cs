using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class SceneFolder : MonoBehaviour
    {
        // static Preference<bool> _alwaysShowSceneFolderGizmos = new Preference<bool>("ShowSceneFolderGizmos", true);
       [SerializeField]  private bool _alwaysShowSceneFolderGizmos;
        
        [SerializeField, CreateAssetButton] private SceneFolderAsset _folderAsset;

        private static Dictionary<SceneFolderAsset, SceneFolder> _sceneFolders = new Dictionary<SceneFolderAsset, SceneFolder>();
        
        [SerializeField] Vector3 _offset = new Vector3(0,0,0);
        [SerializeField] Vector3 _dimensions = new Vector3(1,1,1)*Mathf.Infinity;
        private List<GameObject> _objectsToEncapsulate;


        public static SceneFolder TryGetSceneFolder(SceneFolderAsset folderAsset)
        {
            if (folderAsset == null)
            {
                return null;
            }
            if (_sceneFolders.ContainsKey(folderAsset))
            {
                SceneFolder folder = _sceneFolders[folderAsset];
                if (folder != null)
                {
                    return folder;
                }

            }

            SceneFolder[] findObjectsOfType = CompaitibilityHelper.FindObjectsByType<SceneFolder>();
            foreach (SceneFolder sceneFolder in findObjectsOfType)
            {
                if (sceneFolder._folderAsset == folderAsset)
                {
                    _sceneFolders[folderAsset] = sceneFolder;
                    return sceneFolder;
                }
            }
            return null;
        }

        [Button]
        public void MoveToTopOfChildren()
        {
            RuntimeEditorHelper.RecordObjectUndo(transform);
            transform.SetSiblingIndex(0);
        }

        [Button]
        public void ZeroTransform()
        {
            RuntimeEditorHelper.ZeroPositionWithoutMovingChildren(transform);
        }


        [Button]
        public void CreateEmptyChild()
        {
            GameObject o = new GameObject("Child");
            o.transform.SetParent(transform);
            o.transform.localPosition = Vector3.zero;
            o.transform.SetSiblingIndex(0);
        }

        private void OnDrawGizmosSelected()
        {
            if (_alwaysShowSceneFolderGizmos == false)
            {

                Gizmos.color = Color.yellow;
                Bounds bounds = GetBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);

                if (_objectsToEncapsulate != null)
                {
                    foreach (GameObject gameObject in _objectsToEncapsulate)
                    {
                        Gizmos.DrawCube(gameObject.transform.position, 0.1f * Vector3.one);
                    }
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (_alwaysShowSceneFolderGizmos == true)
            {

                Gizmos.color = Color.yellow;
                Bounds bounds = GetBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);

                if (_objectsToEncapsulate != null)
                {
                    foreach (GameObject gameObject in _objectsToEncapsulate)
                    {
                        Gizmos.DrawCube(gameObject.transform.position, 0.1f * Vector3.one);
                    }
                }
            }
        }


        [Button]
        private void FindItemsToEncapsulate()
        {
            // find all overlapping root objects in the scene by checking it transforms lie within the bounds
            Bounds bounds = GetBounds();
            GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
            _objectsToEncapsulate = new List<GameObject>();
            foreach (GameObject rootObject in rootObjects)
            {
                if (rootObject == gameObject) 
                    continue;

                if (rootObject.HasComponent<SceneFolder>())
                {
                    continue;
                }
                if (bounds.Contains(rootObject.transform.position))
                {
                    Debug.Log(rootObject,rootObject);
                    _objectsToEncapsulate.Add(rootObject);
                }
            }
        }

        private Bounds GetBounds()
        {
            return new Bounds(transform.position+ Vector3.up*(0.5f*_dimensions.y)+_offset , _dimensions );
        }

        [Button]
        private void EncapsulateItems()
        {
            if (_objectsToEncapsulate == null || _objectsToEncapsulate.Count == 0)
            {
                FindItemsToEncapsulate();
            }

            RuntimeEditorHelper.RecordObjectUndo(transform);
            foreach (GameObject gameObject in _objectsToEncapsulate)
            {
                RuntimeEditorHelper.RecordObjectUndo(gameObject.transform);
                RuntimeEditorHelper.RecordSetTransformParent(gameObject.transform, transform);
            }
        }
        
        [Button]
        private void EncapsulateAnyUnparentedItems()
        {
            GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootObjects)
            {
                if (rootObject == gameObject) 
                    continue;

                if (rootObject.HasComponent<SceneFolder>())
                {
                    continue;
                }
                if (rootObject.transform.parent != null)
                {
                    continue;
                }
                Debug.Log(rootObject, rootObject);
                RuntimeEditorHelper.RecordSetTransformParent(rootObject.transform, transform);
            }
        }


        [Button]
        private void EjectChildren()
        {
            List<Transform> directChildren = transform.GetDirectChildren();
            for (var i = directChildren.Count - 1; i >= 0; i--)
            {
                var child = directChildren[i];
                RuntimeEditorHelper.RecordSetTransformParent(child, null);
            }
        }
    }
}
