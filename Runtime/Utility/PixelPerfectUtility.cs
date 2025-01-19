using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    public static class PixelPerfectUtility
    {
       
        public static void SnapSpriteToGrid(GameObject selectedObject)
        {
            SpriteRenderer spriteRenderer = selectedObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                SnapSpriteToGrid(spriteRenderer);
            }
            else
            {
                var pixelSnappingElement = selectedObject.GetComponent<PixelSnappingElement>();
                if (pixelSnappingElement && pixelSnappingElement.dontSnap == false)
                {
                    SnapTransformToGrid(pixelSnappingElement);
                }
            }
        }

        public static void SnapTransformToGrid(PixelSnappingElement element)
        {
            float pixelsPerUnit = 16;
            float unitPerPixel = 1f / pixelsPerUnit;
            Vector3 position = element.transform.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);
            element.transform.position = position;
        }
        
        public static void SnapSpriteToGrid(SpriteRenderer spriteRenderer)
        {
            if (spriteRenderer.sprite == null)
            {
                return;
            }

            var pixelSnappingElement = spriteRenderer.GetComponent<PixelSnappingElement>();
            if (pixelSnappingElement && pixelSnappingElement.dontSnap)
            {
                return;
            }
            
            Transform transform = spriteRenderer.transform;
            float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
            float unitPerPixel = 1f / pixelsPerUnit;

            Vector3 position = transform.position;
            
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size * pixelsPerUnit;

            float xOffset = spriteRenderer.sprite.pivot.x;
            float yOffset = spriteRenderer.sprite.pivot.y;
            // Vector3 pivot = new Vector3(xOffset, y)

            // Debug.Log( $"spriteRenderer.sprite.pivot: {spriteRenderer.sprite.pivot}, yOffset: {yOffset}");
            // Debug.Log( $"xOffset: {xOffset}, yOffset: {yOffset}");

            // Adjust position to account for odd/even dimensions
            // xOffset = (Mathf.Floor(xOffset) % 2 == 0) ? 0f : unitPerPixel / 2f;
            // yOffset = (Mathf.Floor(yOffset) % 2 == 0) ? 0f : unitPerPixel / 2f;

            // Debug.Log( $" Sprite Size {spriteSize} xOffset: {xOffset}, yOffset: {yOffset}");
            
            // Debug.Log($"Round {position.x- xOffset} {position.y - yOffset}");
            // Debug.Log($"Add { xOffset} { yOffset}");
            // Snap each axis
            Vector3 pixelPos = position / unitPerPixel;
            // Debug.Log($"init position {pixelPos}");

            pixelPos.x = Mathf.Round((pixelPos.x - xOffset) )  + xOffset;
            pixelPos.y = Mathf.Round((pixelPos.y - yOffset)) + yOffset;
            // Debug.Log($"position { position/unitPerPixel}");
            Vector3 worldPos = pixelPos*unitPerPixel;

            RuntimeEditorHelper.RecordObjectUndo(transform,"snap");
            transform.SetXY(worldPos); ;
        }
    }
}