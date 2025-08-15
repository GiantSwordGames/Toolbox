using System;
using NaughtyAttributes;
using RichardPieterse;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;

namespace GiantSword
{
    
    
    [ExecuteInEditMode]
    public class TextContentFitter : MonoBehaviour
    {
        [SerializeField] private Vector2 _margin;

        private void OnValidate()
        {
            
            Apply();
        }

        void Start()
        {
            Apply();
        }

        [Button]
        public void Apply()
        {
            if(enabled == false)
            {
                return;
            }
            
            CanvasUtility.FitRectTransformToContainedText(gameObject, _margin);
        }

        public static void RecalculateChildren(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            
            TextContentFitter[] fitters = gameObject.GetComponentsInChildren<TextContentFitter>(true);
            foreach (TextContentFitter fitter in fitters)
            {
                fitter.Apply();
            }
            
        }
    }
}
