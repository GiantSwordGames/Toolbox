using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JamKit
{
    public class FlashGraphicColor : MonoBehaviour
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Gradient _color;
        [SerializeField] private float _duration = 1;
        private Coroutine _coroutine;
        private Color _originalColor;
        private bool _isRunning = false;

        [SerializeField] private UnityEvent _onBegin;
        

        [Button]
        public void Begin()
        {
            if (_isRunning == false)
            {
                Stop();
                _coroutine = StartCoroutine(Flash());
                
                _onBegin?.Invoke();
            }
        }

        [Button]
        public void Stop()
        {
            _isRunning = false;

            if (_coroutine != null)
            {
                _graphic.color = _originalColor;
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }


        private IEnumerator Flash()
        {
            _isRunning = true;
            _originalColor = _graphic.color;
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                time %= _duration;
                _graphic.color = _color.Evaluate(time / _duration);
                yield return null;
            }
        }

    }
}
