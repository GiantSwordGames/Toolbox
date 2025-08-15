using JamKit;
using UnityEngine;

namespace JamKit
{
    public class TriggerTransitionEffect : MonoBehaviour
    {
        [SerializeField] private TransitionBase _transitionPrefab;

        public void Trigger()
        {
            TransitionBase transitionWithAnimation = _transitionPrefab.Instantiate();
            transitionWithAnimation.DoFullTransition(null, null);
        }
    }
}