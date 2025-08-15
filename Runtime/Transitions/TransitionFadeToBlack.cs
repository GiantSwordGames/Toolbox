using System;
using System.Collections;
using UnityEngine;

namespace JamKit
{
    public class TransitionFadeToBlack : MonoBehaviour
    {
         [SerializeField] private CanvasGroup _canvasGroup;
        private IEnumerator IEFadeToBlack(float duration, Action onComplete)
        {

            float lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.deltaTime/duration;
                lerp = Mathf.Clamp01(lerp);
                _canvasGroup.alpha = lerp;
                yield return null;
            }
            onComplete?.Invoke();
        }
        
        private IEnumerator IEFadeFromBlack(float duration, Action onComplete)
        {
            float lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.deltaTime/duration;
                lerp = Mathf.Clamp01(lerp);
                _canvasGroup.alpha = 1 - lerp;
                yield return null;
            }
            onComplete?.Invoke();
        }

        private IEnumerator IEDoFadeTransition(float fadeDuration, Action transitionAction,
            IEnumerator transitionCoroutine)
        {
            DontDestroyOnLoad(gameObject);

            yield return StartCoroutine(IEFadeToBlack( fadeDuration, null));
            yield return new WaitForSeconds(.1f);

            transitionAction?.Invoke();
            if (transitionCoroutine != null)
            {
                yield return  StartCoroutine(transitionCoroutine);
            }
            
            yield return new WaitForSeconds(.1f);
            yield return  StartCoroutine(IEFadeFromBlack( fadeDuration, null));
            yield return null;
            Destroy(this.gameObject);
        }
         
        public static void DoFadeTransition(float fadeDuration, Action transitionAction, IEnumerator transitionCoroutine)
        {
            TransitionFadeToBlack transitionFadeToBlack = GetNewInstance();
            transitionFadeToBlack.StartCoroutine(transitionFadeToBlack.IEDoFadeTransition(fadeDuration, transitionAction, transitionCoroutine));
        }

        public static TransitionFadeToBlack GetNewInstance()
        {
            GameObject prefab  = Resources.Load("Prefab_FadeToBlack") as GameObject;
            GameObject instantiate = Instantiate(prefab);
            TransitionFadeToBlack transitionFadeToBlack = instantiate.GetComponent<TransitionFadeToBlack>();
            return transitionFadeToBlack;
        }
    }
}
