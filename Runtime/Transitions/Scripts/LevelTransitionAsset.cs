using UnityEngine;

namespace GiantSword
{
    public class LevelTransitionAsset : MonoBehaviour
    {
        [SerializeField] private Level level;
        [SerializeField]private TransitionWithAnimation _transition;
        
        public void InstantiateAndDoTransition()
        {
            _transition.InstantiateAndDoLevelTransition(level);
        }
        
    }
}