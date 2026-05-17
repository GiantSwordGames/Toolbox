using System;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;

namespace JamKit
{
    
    
    [ExecuteInEditMode]
    public class TrimRectToFitText : MonoBehaviour
    {
        [SerializeField] private Vector2 _margin;

        private void Reset()
        {
            Apply();
        }
        

        // void Start()
        // {
        //     Apply();
        // }
        //

        private void Update()
        {
            Apply();
        }

        [ContextMenu("Apply")]
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
            
            TrimRectToFitText[] fitters = gameObject.GetComponentsInChildren<TrimRectToFitText>(true);
            foreach (TrimRectToFitText fitter in fitters)
            {
                fitter.Apply();
            }
            
        }
    }
}
