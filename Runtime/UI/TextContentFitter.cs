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
            
            CanvasUtility.FitRectTransformToContainedText(gameObject);
        }

    }
}
