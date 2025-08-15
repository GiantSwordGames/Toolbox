using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class TriggerAnimation : MonoBehaviour
    { 
        private enum Type
        {
            Trigger,
            Bool
        }
        [SerializeField] private Animator _animator;

        [AnimatorParam("_animator")]
        [SerializeField] private string _parameter;
        [SerializeField] private Type _type;
        [SerializeField] private bool _value;
        [SerializeField] private AnimationClip _waitForClip;
        [SerializeField] private UnityEvent _onClipTimeElapsed;
         private Coroutine _IEWaitForClip;
        
        
        [Button]
        public void Trigger()
        {
            

            switch (_type)
            {
                case Type.Trigger:
                    _animator.SetTrigger(_parameter);
                    break;
                case Type.Bool:
                    _animator.SetBool(_parameter, _value);
                    break;
            }

            if (_waitForClip)
            {
                if (_IEWaitForClip != null)
                {
                    StopCoroutine(_IEWaitForClip);
                }
                _IEWaitForClip = StartCoroutine(IEWaitForClip());

            }
           
        }

        private IEnumerator IEWaitForClip()
        {
            yield return new WaitForSeconds(_waitForClip.length);

            _onClipTimeElapsed?.Invoke();
        }
    }
}
