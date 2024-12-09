using System;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;

namespace GiantSword
{
    [ExecuteInEditMode]
    public class TextContentFitter : MonoBehaviour
    {
        void Awake()
        {
            AdjustRectTransform();

        }

        private void OnTextChanged(Object obj)
        {
            AdjustRectTransform();
        }

        private void AdjustRectTransform()
        {
            TextMeshProUGUI   textMesh = GetComponent<TextMeshProUGUI>();
            if (textMesh == null)
            {
                textMesh = GetComponentInChildren<TextMeshProUGUI>();
            }
            RectTransform  rectTransform = GetComponent<RectTransform>();
            // Update layout to get accurate text bounds
            textMesh.ForceMeshUpdate();
            Vector2 textSize = textMesh.textBounds.size;

            // Adjust based on the current anchor/stretch settings
            Vector2 newSizeDelta = rectTransform.sizeDelta;

            if (rectTransform.anchorMin.x != rectTransform.anchorMax.x)
            {
                // RectTransform is set to stretch horizontally; adjust height only
                newSizeDelta.y = textSize.y;
            }
            else if (rectTransform.anchorMin.y != rectTransform.anchorMax.y)
            {
                // RectTransform is set to stretch vertically; adjust width only
                newSizeDelta.x = textSize.x;
            }
            else
            {
                // RectTransform is not stretching; adjust both width and height
                newSizeDelta = textSize;
            }

            rectTransform.sizeDelta = newSizeDelta;
        }

        private void OnDrawGizmosSelected()
        {
            AdjustRectTransform();
        }
    }
}
