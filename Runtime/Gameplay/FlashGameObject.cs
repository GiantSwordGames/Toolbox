using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class FlashGameObject : MonoBehaviour
    {
        [SerializeField] private bool _useUnscaledTime;
        [SerializeField] private GameObject _gameObject;

        [SerializeField] private float _duration = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            _gameObject.SetActive(false);
        }
        
        [Button]
        public void Trigger()
        {
            StopAllCoroutines();
            StartCoroutine(IETrigger());
        }

        private IEnumerator IETrigger()
        {
            _gameObject.SetActive(true);
            if (_useUnscaledTime)
            {
                yield return new WaitForSeconds(_duration);
            }
            else
            {
                yield return new WaitForSecondsRealtime(_duration);
            }
            _gameObject.SetActive(false);
        }
    }
}
