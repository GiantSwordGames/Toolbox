using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class CompleteLevel : MonoBehaviour
    {
        [SerializeField] private ScriptableBool _newGamePlus;
        [FormerlySerializedAs("_levelTransitionReturnToMenu")] [SerializeField] private LevelTransitionAction _levelTransitionActionReturnToMenu;
        [FormerlySerializedAs("_levelTransitionNewGamePlus")] [SerializeField] private LevelTransitionAction _levelTransitionActionNewGamePlus;

        [Button]
        public void Trigger()
        {
            if (_newGamePlus.value ==false)
            {
                _newGamePlus.value = true;
                _levelTransitionActionNewGamePlus.Trigger();
            }
            else
            {
                _newGamePlus.value = false;
                _levelTransitionActionReturnToMenu.Trigger();
            }
        }
    }
}