using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    
    [RequireComponent(typeof(CanvasGroup))]
    public class ToggleUiVisibility : MonoBehaviour
    {
        [SerializeField] private bool _isVisible;
       
        [Button]
        public void Toggle()
        {
            _isVisible = !_isVisible;
            Refresh();
        }
        
        [Button]
        public void ToggleOn()
        {
            _isVisible = true;
            RuntimeEditorHelper.SetDirty(this);
            Refresh();
        }
        
        [Button]
        public void ToggleOff()
        {
            _isVisible = false;
            Refresh();
        }
        
        [Button]
        public void Solo()
        {
            foreach (Transform sibling in transform.parent)
            {
                ToggleUiVisibility uiVisibility = sibling.GetComponent<ToggleUiVisibility>();
                if (uiVisibility)
                {
                    if (transform == uiVisibility.transform)
                    {
                        uiVisibility.ToggleOn();
                    }
                    else
                    {
                        uiVisibility.ToggleOff();
                    }
                }
            }
        }
        

        private void Refresh()
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (_isVisible ==false)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}
