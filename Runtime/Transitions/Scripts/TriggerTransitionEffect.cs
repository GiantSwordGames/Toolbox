using GiantSword;
using UnityEngine;

namespace GiantSword
{
    public class TriggerTransitionEffect : MonoBehaviour
    {
        [SerializeField] private TransitionWithAnimation _transitionPrefab;

        public void Trigger()
        {
            TransitionWithAnimation transitionWithAnimation = _transitionPrefab.Instantiate();
            transitionWithAnimation.DoFullTransition(null, null);
        }
    }
}