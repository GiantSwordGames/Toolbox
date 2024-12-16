using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GiantSword
{
    [InitializeOnLoad]
    public class PixelPerfectSnapper
    {
        private static Preference<bool> _enabled = new Preference<bool>("PixelPerfectSnapperEnabled", false);
        static PixelPerfectSnapper()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            DeveloperPreferences.RegisterPreference(_enabled);
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (_enabled.value == false)
            {
                return;
            }
            
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                SnapSelectedSpriteRenderersToGrid();
            }

            if (Event.current.type == EventType.KeyDown)
            {
                HandleArrowKeyNudge();
            }
        }

        private static void SnapSelectedSpriteRenderersToGrid()
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                SpriteRenderer spriteRenderer = selectedObject.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    SnapSpriteToGrid(spriteRenderer);
                }
            }
        }

        private static void SnapSpriteToGrid(SpriteRenderer spriteRenderer)
        {
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

        private static void HandleArrowKeyNudge()
        {
            float nudgeAmount = 0f;

            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                SpriteRenderer spriteRenderer = selectedObject.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
                    float unitPerPixel = 1f / pixelsPerUnit;

                    Vector3 position = selectedObject.transform.position;

                    if (Event.current.keyCode == KeyCode.UpArrow)
                    {
                        position.y += unitPerPixel;
                        nudgeAmount = unitPerPixel;
                    }
                    else if (Event.current.keyCode == KeyCode.DownArrow)
                    {
                        position.y -= unitPerPixel;
                        nudgeAmount = unitPerPixel;
                    }
                    else if (Event.current.keyCode == KeyCode.LeftArrow)
                    {
                        position.x -= unitPerPixel;
                        nudgeAmount = unitPerPixel;
                    }
                    else if (Event.current.keyCode == KeyCode.RightArrow)
                    {
                        position.x += unitPerPixel;
                        nudgeAmount = unitPerPixel;
                    }

                    selectedObject.transform.position = position;
                }
            }

            if (nudgeAmount > 0f)
            {
                Event.current.Use(); // Mark event as used to prevent other handlers
            }
        }
    }
}
