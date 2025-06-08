using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace GiantSword
{
    public abstract class TransitionBase : MonoBehaviour
    {
        [SerializeField] protected float _startDelay = 0f;
        [FormerlySerializedAs("_holdAtApex")] [SerializeField] protected float _hold = 0f;
        private bool _doNotAutoDestroy;

        protected virtual void OnValidate()
        {
        }

        [Button]
        public void Trigger()
        {
            InstantiateAndDoFullTransition();
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
            if (_doNotAutoDestroy == false && Application.isPlaying)
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
        
        public TransitionBase InstantiateAndDoLevelTransition(Level leve)
        {
            var instance = Instantiate();
            instance.DoFullTransition(() => leve.LoadLevel(), null);
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
    }
}