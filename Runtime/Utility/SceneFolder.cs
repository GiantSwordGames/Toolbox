using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class SceneFolder : MonoBehaviour
    {
        [SerializeField, CreateAssetButton] private SceneFolderAsset _folderAsset;

        private static Dictionary<SceneFolderAsset, SceneFolder> _sceneFolders = new Dictionary<SceneFolderAsset, SceneFolder>();
        
        
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

            SceneFolder[] findObjectsOfType = FindObjectsOfType<SceneFolder>();
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
    }
}
