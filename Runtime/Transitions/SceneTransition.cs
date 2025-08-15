using GiantSword;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GiantSword
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private SceneReference _sceneToLoad;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private TransitionBase _transitionBase;


        private bool _triggered = false;

        private void Trigger2()
        {
            Debug.Log("Triggered");
        }

        public void Trigger()
        {
            if (_triggered == false)
            {
                _triggered = true;
                if (_transitionBase)
                {
                    _transitionBase.InstantiateAndDoSceneTransition(_sceneToLoad);
                }
                else
                {
                    TransitionFadeToBlack.DoFadeTransition(_fadeDuration,
                        () => { SceneManager.LoadScene(_sceneToLoad.SceneName); }, null);
                }
            }

        }
    }
}