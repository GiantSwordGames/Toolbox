using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public class SceneFolder : MonoBehaviour
    {
        [SerializeField] private SceneFolderAsset _folderAsset;

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
    }
}
