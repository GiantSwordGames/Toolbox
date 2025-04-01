using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
   
    public class DelayedActionList : MonoBehaviour
    {
        [Serializable]
        struct Entry
        {
            public float delay;
            public UnityEvent UnityEvent;
        }
       
        
        [SerializeField] private TimeScale _timeScale;
        
        [SerializeField] private List<Entry> _entries = new List<Entry>();

        [Button]
        public void Trigger()
        {
            StartCoroutine(IETrigger());
        }

        private IEnumerator IETrigger()
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].delay > 0)
                {
                    yield return TimeHelper.SmartWaitForSeconds(_entries[i].delay, _timeScale);
                }
                
                _entries[i].UnityEvent?.Invoke();
            }
        }

     
    }
}