using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JamKit
{
    [InitializeOnLoad]
    public class PixelPerfectAutoSnapper
    {
        private static Preference<bool> _enabled = new Preference<bool>("PixelPerfectSnapperEnabled", false, PreferenceMode.Project, true);
        static PixelPerfectAutoSnapper()
        {
            #if UNITY_EDITOR
            SceneView.duringSceneGui += OnSceneGUI;
            #endif
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
            #if UNITY_EDITOR
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                PixelPerfectUtility.SnapSpriteToGrid(selectedObject);
            }
#endif
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
