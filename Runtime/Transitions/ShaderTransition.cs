using System;
using System.Collections;
using JamKit;
using UnityEngine;
using UnityEngine.UI;

public class ShaderTransition : TransitionBase
{
    [SerializeField] private float _durationIn = 1;
    [SerializeField] private float _durationOut =1;
    
    [SerializeField] private float _holdBeforeAction = 0;
    [SerializeField] private float _holdAfterAction = 0;
    [SerializeField] private Image _image;


    private Material runtimeMaterial;

    void Awake()
    {
        runtimeMaterial = Instantiate(_image.material);
        _image.material = runtimeMaterial;
    }

    void SetLerp()
    {
      
    }

   

    
        protected override IEnumerator IETransitionIn(Action onComplete)
        {
            if (_startDelay > 0)
            {
                yield return new WaitForSecondsRealtime(_startDelay);
            }
            _onTransitionInBegin?.Invoke();

            float lerp = 0;
            if (_durationIn > 0)
            {
                while (lerp < 1)
                {
                    lerp += Time.unscaledDeltaTime / _durationIn;
                    lerp = Mathf.Clamp01(lerp);
                    runtimeMaterial.SetFloat("_MinCutoff",  1- lerp );
                    runtimeMaterial.SetFloat("_MaxCutoff", 1);
                    yield return null;
                }
            }


            onComplete?.Invoke();
        }

        protected override IEnumerator IETransitionOut(Action onComplete)
        {
            
            _onTransitionOutBegin?.Invoke();
            float lerp = 0;
            if (_durationOut > 0)
            {
                while (lerp < 1)
                {
                    lerp += Time.unscaledDeltaTime / _durationOut;
                    lerp = Mathf.Clamp01(lerp);
                    runtimeMaterial.SetFloat("_MinCutoff",  0 );
                    runtimeMaterial.SetFloat("_MaxCutoff", 1- lerp);
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
            yield return null;

            yield return DoTransitionIn();
            yield return null;
            if(_holdAfterAction > 0)
            {
                yield return new WaitForSecondsRealtime(_holdAfterAction);
            }
            
            float holdStart = Time.realtimeSinceStartup;

            onTransitionInComplete?.Invoke();
            yield return null;
            yield return null;

            while (Time.realtimeSinceStartup - holdStart < _hold)
            {
                // _hold -= Time.unscaledTime;
                yield return null;
            }
            yield return null;
            if (_holdBeforeAction > 0)
            {
                yield return new WaitForSecondsRealtime(_holdBeforeAction);
            }
            yield return DoTransitionOut();
            yield return null;
            onTransitionOutComplete?.Invoke();
            yield return null;

            if (_autoDestroy)
            {
                Destroy(this.gameObject);
            }
        }

}
