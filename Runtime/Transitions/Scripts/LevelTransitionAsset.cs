using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class LevelTransitionAsset : MonoBehaviour
    {
        [SerializeField] private Level level;
        
        [Button]
        public void InstantiateAndDoTransition()
        {
            TransitionBase transitionBase = GetComponent<TransitionBase>();
            transitionBase.InstantiateAndDoLevelTransition(level);
        }
        
    }
}