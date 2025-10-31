using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class RevealChildrenOverTime : MonoBehaviour
    {
        [SerializeField] private float _interval = 0.2f;
        [SerializeField] private UnityEvent _onChildReveal;
        private Coroutine _coroutine;

        private void Awake()
        {
            Reset();
        }

        [NaughtyAttributes.Button]
        public void Trigger()
        {
            IETrigger();
        }

        public void Reset()
        {
            if (_coroutine != null)
            {
                AsyncHelper.StopRoutine(_coroutine);
                _coroutine = null;
            }
            
            List<Transform> children = transform.GetDirectChildren();
            for (int j = 0; j < children.Count; j++)
            {
                children[j].gameObject.SetActive(false);
            }

        }

        public Coroutine IETrigger()
        {
            _coroutine = AsyncHelper.StartCoroutine(IEReveal());
            return _coroutine;
        }
        
        private IEnumerator IEReveal()
        {
            
            List<Transform> children = transform.GetDirectChildren();
            

            for (int i = 0; i < children.Count; i++)
            {
                for (int j = 0; j < children.Count; j++)
                {
                    children[j].gameObject.SetActive(i >= j);
                    _onChildReveal?.Invoke();
                }
                yield return new WaitForSeconds(_interval);
            }
        }
    }
}
