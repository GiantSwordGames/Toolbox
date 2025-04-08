using System.Collections;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GiantSword
{
    public class LevelTransitionAction : MonoBehaviour
    {
        [SerializeField] private ScriptableVariableScope _resetVariableScope = ScriptableVariableScope.Undefined;
        [SerializeField] bool _restartLevel;
        [SerializeField] bool _goToNextLevelInBuildSettings;
        [SerializeField] private Level _level;
        [SerializeField] private TransitionWithAnimation _transition;
        [SerializeField] private TransitionBase _transitionBase;
        [SerializeField] private float _delay = 0;

        [Button]
        public void Trigger()
        {
            AsyncHelper.StartCoroutine(IETrigger());
        }
        public IEnumerator IETrigger()
        {
            if (_transition)
            {
                _transitionBase = _transition;
            }
            if (_delay > 0)
            {
                yield return new WaitForSecondsRealtime(_delay);
            }
            
            ScriptableVariableManager.ResetAll(_resetVariableScope);
            if (_restartLevel)
            {
                _transitionBase.InstantiateAndDoSceneTransition( SceneManager.GetActiveScene().buildIndex);
            }
            else if (_goToNextLevelInBuildSettings)
            {
               // get thte current scene index
                int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
                int nextSceneIndex = currentSceneBuildIndex + 1;
                nextSceneIndex %= SceneManager.sceneCountInBuildSettings;
                _transitionBase.InstantiateAndDoSceneTransition(nextSceneIndex);

            }
            else
            {
                _transitionBase.InstantiateAndDoLevelTransition(_level);
            }
        }
        
    }
}