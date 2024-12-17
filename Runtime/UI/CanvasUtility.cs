using TMPro;
using UnityEngine;

namespace RichardPieterse
{
    public static class CanvasUtility 
    {
        public static void FitRectTransformToContainedText(this GameObject gameObject)
        {
            
            TextMeshProUGUI   textMesh = gameObject.GetComponent<TextMeshProUGUI>();
            if (textMesh == null)
            {
                textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            }
            RectTransform  rectTransform = gameObject.GetComponent<RectTransform>();
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

    }
}
