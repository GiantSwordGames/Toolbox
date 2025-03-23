using System;
using System.Collections;
  using GiantSword;
  using NaughtyAttributes;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  namespace GiantSword
{
    public class TransitionWithAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _inDuration;
        [SerializeField] private AnimationClip _outDuration;
        [SerializeField] private float _startDelay = 0f;
        [SerializeField] private float _holdAtApex = 0f;
        private static readonly int InTrigger = Animator.StringToHash("TransitionIn");
        private static readonly int OutTrigger = Animator.StringToHash("TransitionOut");

        private bool _doNotAutoDestroy;


        private void OnValidate()
        {
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        [Button]
        public void Trigger()
        {
            DoFullTransition(null, null);
        }
        
        private IEnumerator IETransitionIn(Action onComplete)
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

        private IEnumerator IETransitionOut(Action onComplete)
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

        private IEnumerator IEDoFullTransition(Action onTransitionInComplete,
            Action onTransitionOutComplete)
        {
            if(_doNotAutoDestroy ==false)
                DontDestroyOnLoad(gameObject);
            
            yield return DoTransitionIn();
            yield return null;
            Debug.Log("Transition In Complete");
            Debug.Log(Time.frameCount + " Transition In Complete " + Time.time);

            onTransitionInComplete?.Invoke();
            yield return null;

            if (_holdAtApex > 0)
            {
                yield return new WaitForSeconds(_holdAtApex);
            }
            Debug.Log(Time.frameCount + " Transition  Out begin " + Time.time);
            yield return DoTransitionOut();
            yield return null;
            onTransitionOutComplete?.Invoke();
            yield return null;
            Debug.Log(Time.frameCount + " Transition  Out Complete " + Time.time);
            
            if(_doNotAutoDestroy == false)
                Destroy(this.gameObject);
        }

        private Coroutine DoTransitionOut()
        {
            return AsyncHelper.StartCoroutine(IETransitionOut(null));
        }

        private Coroutine DoTransitionIn()
        {
            return AsyncHelper.StartCoroutine(IETransitionIn(null));
        }

        [Button]
        public void TestTransition()
        {
            _doNotAutoDestroy = true;
            DoFullTransition(null, null);
        }

        public Coroutine DoFullTransition(Action onTransitionInComplete, Action onTransitionOutComplete)
        {
            return AsyncHelper.StartCoroutine(IEDoFullTransition(onTransitionInComplete, onTransitionOutComplete));
        }

        public TransitionWithAnimation Instantiate()
        {
            GameObject newInstance = Instantiate(gameObject);
            TransitionWithAnimation transitionFadeToBlack = newInstance.GetComponent<TransitionWithAnimation>();
            return transitionFadeToBlack;
        }
        
        public TransitionWithAnimation InstantiateAndDoSceneTransition( int buildIndex)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => SceneManager.LoadScene(buildIndex), null);  
            return instance;
        }
        public TransitionWithAnimation InstantiateAndDoFullTransition(Action onTransitionInComplete,
            Action onTransitionOutComplete)
        {
            var instance = Instantiate();
            instance.DoFullTransition(onTransitionInComplete, onTransitionOutComplete);
            return instance;
        }
        
        public TransitionWithAnimation InstantiateAndDoLevelTransition(Level loadScene)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => loadScene.LoadLevel(), null);
            return instance;
        }
        
        public TransitionWithAnimation InstantiateAndDoFullTransition()
        {
            var instance = Instantiate();
            instance.DoFullTransition(null, null);
            return instance;
        }
        
        public void InstantiateAndDoTransitionIn()
        {
            var instance = Instantiate();
            instance.DoTransitionIn();
        }

        public TransitionWithAnimation InstantiateAndDoSceneTransition(SceneReference sceneReference)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => sceneReference.Load(), null);  
            return instance;
        }
    }
}
