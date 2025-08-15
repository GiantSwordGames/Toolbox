using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class EnableChildrenSequentially : MonoBehaviour
    {
        [SerializeField] Transform _parent;
        [SerializeField] bool _randomizeOrder =false;
        [SerializeField] float _floatInterval = 0.1f;
        [SerializeField] SmartFloat _maxCount = new SmartFloat( 1000000);

        [SerializeField] private UnityEvent _onComplete;
        void Start()
        {
            for (int i = 0; i < _parent.childCount; i++)
            {
                _parent.GetChild(i).gameObject.SetActive(false);
            }

            Trigger();
        }

        public void Trigger()
        {
            StartCoroutine(IETrigger());
        }

        // Update is called once per frame
        IEnumerator IETrigger()
        {
            List<Transform> directChildren = _parent.GetDirectChildren<Transform>();

            if (_randomizeOrder)
            {
                directChildren.Shuffle();
            }

            for (int i = 0; i < directChildren.Count; i++)
            {
                if (i >= _maxCount)
                {
                    break;
                }
                directChildren[i].gameObject.SetActive(true);
                if (_floatInterval > 0)
                    yield return new WaitForSeconds(_floatInterval);
            }

            _onComplete.Invoke();
        }
    }
}
