using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class RevealChildren : MonoBehaviour
    {
        [SerializeField] private float _interval = 0.2f;
        private Coroutine _coroutine;

        private void Awake()
        {
            Reset();
        }

        [Button]
        public void Trigger()
        {
            IETrigger();
        }

        public void Reset()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
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
            _coroutine = StartCoroutine(IEReveal());
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
                }
                yield return new WaitForSeconds(_interval);
            }
        }
    }
}
