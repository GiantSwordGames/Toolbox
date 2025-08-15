using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class LevelTransitionAsset : MonoBehaviour
    {
        [FormerlySerializedAs("level")] [SerializeField] private Level _level;
        
        [Button]
        public void InstantiateAndDoTransition()
        {
            TransitionBase transitionBase = GetComponent<TransitionBase>();
            transitionBase.InstantiateAndDoLevelTransition(_level);
        }
        
        [Button]
        public void InstantiateAndDoTransition(Level level )
        {
            TransitionBase transitionBase = GetComponent<TransitionBase>();
            transitionBase.InstantiateAndDoLevelTransition(level);
        }

        
    }
}