using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class ReloadLevel : MonoBehaviour
    {

        [SerializeField] private InputKeyAsset _asset;
        [SerializeField] private KeyCode _reloadKey = KeyCode.R;
        
        [Button]
        public void Reload()
        {
            // Reload the current scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_reloadKey))
            {
                Reload();
            }
        }
    }
}
