using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class ActionSequence : MonoBehaviour
    {
        [Serializable]
        public class Entry 
        {
            public SmartFloat _delayBefore;
            public UnityEvent _actions;
        [HideInInspector]    public SmartFloat _delayAfter; // deprecate
        }
        
        [SerializeField] private List<Entry> _entries = new List<Entry>();
        
        [SerializeField] private UnityEvent _onCompleteEvent;
        private Coroutine _IESequence;
        [ShowNonSerializedField] private int _step;

        [Button]
        public void Trigger()
        {
            TriggerReturnRoutine();
        }
        public Coroutine TriggerReturnRoutine()
        {
            if (_IESequence != null)
            {
                StopCoroutine(_IESequence);
            }
            _IESequence = StartCoroutine(IESequence());
            return _IESequence;
        }

        private IEnumerator IESequence()
        {
            _step = 0;
            foreach (var entry in _entries)
            {
                if (entry._delayBefore > 0)
                {
                    yield return new WaitForSecondsRealtime(entry._delayBefore);
                }
                entry._actions.Invoke();
                
                if (entry._delayAfter > 0)
                {
                    yield return new WaitForSecondsRealtime(entry._delayAfter);
                }
                _step++;
            }
            
            _onCompleteEvent.Invoke();
        }
    }
}