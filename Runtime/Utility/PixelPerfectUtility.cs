using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public static class PixelPerfectUtility
    {
       
        public static void SnapSpriteToGrid(GameObject selectedObject)
        {
            SpriteRenderer spriteRenderer = selectedObject.GetComponent<SpriteRenderer>();
            var pixelSnappingElement = selectedObject.GetComponent<PixelSnappingElement>();
            var pixelSnappingParent = selectedObject.GetComponentInParent<PixelSnappingElement>();
         
            {
               
                if (pixelSnappingParent)
                {
                    if (pixelSnappingElement == null)
                    {
                        // dont snap children
                        return;
                    }
                }
                
                if ( pixelSnappingElement && pixelSnappingElement.dontSnap == false)
                {
                    SnapTransformToGrid(pixelSnappingElement);
                    return;
                }
                
                if (spriteRenderer != null)
                {
                    SnapSpriteToGrid(spriteRenderer);
                    return;

                }
            }
        }

        public static void SnapTransformToGrid(PixelSnappingElement element)
        {
            float pixelsPerUnit = 16;

            if (element.snapScale)
            {
                Vector3 scale = element.transform.localScale;
                scale *= pixelsPerUnit;
                scale.x = Mathf.Round(scale.x);
                scale.y = Mathf.Round(scale.y);
                scale /= pixelsPerUnit;
                element.transform.localScale = scale;
                element.pixelSize = element.transform.localScale*pixelsPerUnit;
            }
            
            Vector3 pixelPosition = element.transform.position*pixelsPerUnit;
            Vector2 offset = (element.pixelSize / 2f);
            offset.x %= 1;
            offset.y %= 1;
            
            pixelPosition.x += offset.x;
            pixelPosition.x = Mathf.Round(pixelPosition.x);
            pixelPosition.y = Mathf.Round(pixelPosition.y);
            pixelPosition.x -= offset.x;
            element.pixelPosition = pixelPosition;

            element.transform.position = pixelPosition/pixelsPerUnit;
        }
        
        public static Vector2 Snap(Vector2 worldSpace)
        {
            float pixelsPerUnit = 16;
            Vector3 pixelPosition = worldSpace*pixelsPerUnit;
            pixelPosition.x = Mathf.Round(pixelPosition.x);
            pixelPosition.y = Mathf.Round(pixelPosition.y);
            return pixelPosition/pixelsPerUnit;
        }

        
        public static Vector3 Snap(Vector3 worldSpace)
        {
            float pixelsPerUnit = 16;
            Vector3 pixelPosition = worldSpace*pixelsPerUnit;
            pixelPosition.x = Mathf.Round(pixelPosition.x);
            pixelPosition.y = Mathf.Round(pixelPosition.y);
            return pixelPosition/pixelsPerUnit;
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