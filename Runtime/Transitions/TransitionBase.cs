using System;
using System.Collections;
using Framework;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace JamKit
{
    public abstract class TransitionBase : MonoBehaviour
    {
        [SerializeField] protected bool __dontDestroyOnLoad = true;
        [SerializeField] protected bool _autoDestroy = true;
        [SerializeField] protected bool _playOnStart = false;
        [SerializeField] protected float _startDelay = 0f;
        [FormerlySerializedAs("_holdAtApex")] [SerializeField] protected float _hold = 0f;

        [SerializeField] protected UnityEvent _onTransitionInBegin = default;
        [SerializeField] protected UnityEvent _onTransitionInComplete = default;
        [SerializeField] protected UnityEvent _mainAction = default;
        [SerializeField] protected UnityEvent _onTransitionOutBegin = default;
        [SerializeField] protected UnityEvent _onTransitionOutComplete = default;


        private void Start()
        {
            if (_playOnStart)
            {
                DoFullTransition(null, null);
            }
        }

        protected virtual void OnValidate()
        {
        }

        [Button]
        public void Trigger()
        {
            if (ValidationUtility.IsPrefabAsset(gameObject))
            {
                InstantiateAndDoFullTransition();
            }
            else
            {
                this.DoFullTransition(null, null);
            }
        }

        protected abstract IEnumerator IETransitionIn(Action onComplete);

        protected abstract IEnumerator IETransitionOut(Action onComplete);

        protected abstract IEnumerator IEDoFullTransition(Action onTransitionInComplete,
            Action onTransitionOutComplete);

        protected Coroutine DoTransitionOut()
        {
            return this.StartCoroutine(IETransitionOut(null));
        }

        protected Coroutine DoTransitionIn()
        {
            return this.StartCoroutine(IETransitionIn(null));
        }

        // public void TestAsset()
        // {
        //     InstantiateAndDoFullTransition();
        // }

        public Coroutine DoFullTransition(Action onTransitionInComplete, Action onTransitionOutComplete)
        {
            if (__dontDestroyOnLoad  && Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
            
            return this.StartCoroutine(IEDoFullTransition(onTransitionInComplete, onTransitionOutComplete));
        }

        public TransitionBase Instantiate()
        {
            GameObject newInstance = gameObject.SmartInstantiate();
            TransitionBase transitionFadeToBlack = newInstance.GetComponent<TransitionBase>();
            return transitionFadeToBlack;
        }
        
        public TransitionBase InstantiateAndDoSceneTransition( int buildIndex)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => SceneManager.LoadScene(buildIndex), null);  
            return instance;
        }
        public TransitionBase InstantiateAndDoFullTransition(Action onTransitionInComplete,
            Action onTransitionOutComplete)
        {
            var instance = Instantiate();
            instance.DoFullTransition(onTransitionInComplete, onTransitionOutComplete);
            return instance;
        }
        
        public TransitionBase InstantiateAndDoLevelTransition(Level level)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => level.LoadLevel(true), null);
            return instance;
        }
        
        public TransitionBase InstantiateAndDoFullTransition()
        {
            var instance = Instantiate();
                instance.DoFullTransition(null, null);
            return instance;
        }
        
        private void InstantiateAndDoTransitionIn()
        {
            var instance = Instantiate();
            instance.DoTransitionIn();
        }

        public TransitionBase InstantiateAndDoSceneTransition(SceneReference sceneReference)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => sceneReference.Load(), null);  
            return instance;
        }
        
        
        public TransitionBase InstantiateAndDoSceneTransition(string sceneName)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => SceneManager.LoadScene(sceneName), null);  
            return instance;
        }

        public void SetText(string text)
        {
            TextMeshProUGUI textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshProUGUI != null)
            {
                textMeshProUGUI.text = text;
            }
        }
    }
}