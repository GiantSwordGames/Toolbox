using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class FlashMeshRenderer : MonoBehaviour
    {
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private float _duration = 0.1f;
        [SerializeField] private Renderer[] _renderers;

        private Coroutine _routine;


        [Button]
        public void DoFlash()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(IEFlash());
        }

        private IEnumerator IEFlash()
        {
            float time = 0;
            while (time < _duration)
            {
                time += Time.deltaTime;
                Color color = _gradient.Evaluate(time / _duration);
                SetColor(color);
                yield return null;
            }

            SetColor(_defaultColor);
        }

        private void SetColor(Color color)
        {
            foreach (var r in _renderers)
            {
                r.material.color = color;
            }
        }
    }
}