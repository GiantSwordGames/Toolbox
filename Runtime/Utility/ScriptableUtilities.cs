using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class ScriptableUtilities : ScriptableSingleton<ScriptableUtilities>
    {
        [Button]
        public void FreezeTime()
        {
            TimeHelper.PausePhysics();
        }

        [Button]
        public  void UnfreezeTime()
        {
            TimeHelper.UnpausePhysics();
        }
        
        public  void SlowTime(float timeScale)
        {
            TimeHelper.timeScale = timeScale;
        }
        
        
        public  void Quit()
        {
            // if not web build
            #if !UNITY_WEBGL
            Application.Quit();
            #endif
        }
        public void ToggleFullScreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        
        public void ExitFullScreen()
        {
            Screen.fullScreen = false;
        }
        
        public void EnterFullScreen()
        {
            Screen.fullScreen = true;
        }
        
        [Button]
        public void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}