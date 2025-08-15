using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
   
    public class ProceduralTransition : TransitionBase
    {
        [SerializeField] private Color _color = Color.black;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;
        [SerializeField] private float _durationIn = 1;
        [SerializeField] private float _durationOut =1;
        [SerializeField] private AnimationCurve _curveIn = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _curveOut = AnimationCurve.Linear(0,0,1,1);

        protected override void OnValidate()
        {
            base.OnValidate();
            _image.color = _color;
        }

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
                    _canvasGroup. alpha =  _curveIn.Evaluate(lerp);
                    _image.color = _color;
                    yield return null;
                }
            }

            _image.color = _color;
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
                    _canvasGroup. alpha = 1-_curveOut.Evaluate( lerp);
                    _image.color = _color;
                    yield return null;
                }
            }

            onComplete?.Invoke();
        }

        protected override IEnumerator IEDoFullTransition(Action onTransitionInComplete,
            Action onTransitionOutComplete)
        {
            if (__dontDestroyOnLoad  && Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            yield return DoTransitionIn();
            yield return null;

            onTransitionInComplete?.Invoke();
            yield return null;

            while (_hold > 0)
            {
                _hold -= Time.deltaTime;
                yield return null;
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

        public void Close()
        {
            _hold = 0;
        }
    }
}