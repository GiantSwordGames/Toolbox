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

        [Button]
        public void Trigger()
        {
            ScriptableVariableManager.ResetAll(_resetVariableScope);
            if (_restartLevel)
            {
                _transition.InstantiateAndDoSceneTransition( SceneManager.GetActiveScene().buildIndex);
            }
            else if (_goToNextLevelInBuildSettings)
            {
               // get thte current scene index
                int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
                int nextSceneIndex = currentSceneBuildIndex + 1;
                nextSceneIndex %= SceneManager.sceneCountInBuildSettings;
                _transition.InstantiateAndDoSceneTransition(nextSceneIndex);

            }
            else
            {
                _transition.InstantiateAndDoLevelTransition(_level);
            }
        }
        
    }
}