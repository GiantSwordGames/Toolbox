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
            onTransitionInComplete?.Invoke();
            yield return null;
            yield return DoTransitionOut();
            yield return null;
            onTransitionOutComplete?.Invoke();
            yield return null;
            
            if(_doNotAutoDestroy == false)
                Destroy(this.gameObject);
        }

        private Coroutine DoTransitionOut()
        {
            return StartCoroutine(IETransitionOut(null));
        }

        private Coroutine DoTransitionIn()
        {
            return StartCoroutine(IETransitionIn(null));
        }

        [ContextMenu("Do Transition")]
        public void TestTransition()
        {
            _doNotAutoDestroy = true;
            DoFullTransition(null, null);
        }

        public Coroutine DoFullTransition(Action onTransitionInComplete, Action onTransitionOutComplete)
        {
            return StartCoroutine(IEDoFullTransition(onTransitionInComplete, onTransitionOutComplete));
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
            
            Debug.Log("Load scene " + buildIndex);
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
    }
}
