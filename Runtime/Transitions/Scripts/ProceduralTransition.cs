using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GiantSword
{
   
    public class ProceduralTransition : TransitionBase
    {
        [SerializeField] private Color _color = Color.black;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;
        [SerializeField] private float _durationIn = 1;
        [SerializeField] private float _durationOut =1;
        
        protected override IEnumerator IETransitionIn(Action onComplete)
        {
            if (_startDelay > 0)
            {
                yield return new WaitForSecondsRealtime(_startDelay);
            }

            float lerp = 0;
            if (_durationIn > 0)
            {
                while (lerp < 1)
                {
                    lerp += Time.unscaledDeltaTime / _durationIn;
                    lerp = Mathf.Clamp01(lerp);
                    _canvasGroup. alpha = lerp;
                    _image.color = _color.WithAlpha(1);
                    yield return null;
                }
            }

            _image.color = _color.WithAlpha(1);
            _canvasGroup. alpha = 1;

            onComplete?.Invoke();
        }

        protected override IEnumerator IETransitionOut(Action onComplete)
        {
            float lerp = 0;
            if (_durationOut > 0)
            {
                while (lerp < 1)
                {
                    lerp += Time.unscaledDeltaTime / _durationOut;
                    lerp = Mathf.Clamp01(lerp);
                    _canvasGroup. alpha = 1 - lerp;
                    _image.color = _color.WithAlpha(1);
                    yield return null;
                }
            }

            onComplete?.Invoke();
        }

        protected override IEnumerator IEDoFullTransition(Action onTransitionInComplete,
            Action onTransitionOutComplete)
        {
            DontDestroyOnLoad(gameObject);

            yield return DoTransitionIn();
            yield return null;

            onTransitionInComplete?.Invoke();
            yield return null;

            if (_hold > 0)
            {
                yield return new WaitForSeconds(_hold);
            }

            yield return DoTransitionOut();
            yield return null;
            onTransitionOutComplete?.Invoke();
            yield return null;

            Destroy(this.gameObject);
        }

        [NaughtyAttributes.Button]
        public void SetTransparentInEditMode()
        {
            _canvasGroup.alpha = 0;
        }
    }
}