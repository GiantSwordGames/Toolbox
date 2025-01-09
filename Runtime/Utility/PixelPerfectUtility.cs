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
            position.x = Mathf.Round((position.x) / unitPerPixel) * unitPerPixel;
            position.y = Mathf.Round((position.y) / unitPerPixel) * unitPerPixel;

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

            // Adjust position to account for odd/even dimensions
            float xOffset = (Mathf.Floor(spriteSize.x) % 2 == 0) ? 0f : unitPerPixel / 2f;
            float yOffset = (Mathf.Floor(spriteSize.y) % 2 == 0) ? 0f : unitPerPixel / 2f;

            // Snap each axis
            position.x = Mathf.Round((position.x - xOffset) / unitPerPixel) * unitPerPixel + xOffset;
            position.y = Mathf.Round((position.y - yOffset) / unitPerPixel) * unitPerPixel + yOffset;

            transform.position = position;
        }
    }
}