using System;
using System.Collections;
  using GiantSword;
  using UnityEngine;

  namespace GiantSword
{
    public class TransitionWithAnimation : TransitionBase
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _inDuration;
        [SerializeField] private AnimationClip _outDuration;
        private static readonly int InTrigger = Animator.StringToHash("TransitionIn");
        private static readonly int OutTrigger = Animator.StringToHash("TransitionOut");


        protected override void OnValidate()
        {
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        
        protected override IEnumerator IETransitionIn(Action onComplete)
        {
            yield return new WaitForSecondsRealtime(_startDelay);
            
            _animator.SetTrigger(InTrigger);
            _animator.ResetTrigger(OutTrigger);
            
            float inDurationLength = _inDuration.length + 0.01f;
            float lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.unscaledDeltaTime / inDurationLength;
                lerp = Mathf.Clamp01(lerp);
                yield return null;
            }

            onComplete?.Invoke();
        }

        protected override IEnumerator IETransitionOut(Action onComplete)
        {
            _animator.SetTrigger(OutTrigger);
            _animator.ResetTrigger(InTrigger);
            float outDurationLength = _outDuration.length + 0.01f;
            float lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.unscaledDeltaTime / outDurationLength;
                lerp = Mathf.Clamp01(lerp);
                yield return null;
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

    }
}
