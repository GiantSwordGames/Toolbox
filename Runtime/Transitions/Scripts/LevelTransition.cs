using UnityEngine;

namespace GiantSword
{
    public class LevelTransition : MonoBehaviour
    {
        [SerializeField] private Level level;
        [SerializeField]private TransitionWithAnimation _transition;
        
        public void InstantiateAndDoTransition()
        {
            _transition.InstantiateAndDoLevelTransition(level);
        }
        
    }
}