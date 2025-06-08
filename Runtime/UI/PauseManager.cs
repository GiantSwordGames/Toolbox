using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private InputKeyAsset _pause;
        [SerializeField] private GameObject _pauseMenuRoot;
        [SerializeField] private MenuOptionAsset _resume;
        
        [ShowNonSerializedField] private bool _isPaused;

        private void Start()
        {
            _resume.onClicked += (Resume);
        }
        
        private void OnDestroy()
        {
            _resume.onClicked -= (Resume);
        }

        public void Resume()
        {
            if (_isPaused)
            {
                _isPaused = false;
                _pauseMenuRoot.SetActive(false);
                
                if (Application.isPlaying)
                {
                    TimeHelper.UnpausePhysics();
                }
            }
        }
        
        public void Pause()
        {
            if (_isPaused == false)
            {
                _isPaused = true;
                _pauseMenuRoot.SetActive(true);
                
                if (Application.isPlaying)
                {
                    TimeHelper.PausePhysics();
                }
            }
           
        }


        void Update()
        {
            if (_pause.IsDown())
            {
                if (  _isPaused == false)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }
        }

        [Button]
        private void TogglePause()
        {
            if (_isPaused == false)
            {
                Pause();
            }
            else
            {
                Resume();
            }   
        }
    }
}
