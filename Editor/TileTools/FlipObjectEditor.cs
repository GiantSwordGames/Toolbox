using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    [CustomEditor(typeof(FlipObject))]
    public class FlipObjectEditor : Editor
    {
        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            // Check for key presses
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow)
                {
                    FlipSelectedObjects(true);
                    e.Use(); // Mark the event as used
                }
                else if (e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.DownArrow)
                {
                    FlipSelectedObjects(false);
                    e.Use(); // Mark the event as used
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Flip X"))
            {
                FlipSelectedObjects(true);
            }

            if (GUILayout.Button("Flip Y"))
            {
                FlipSelectedObjects(false);
            }
        }
        
        void FlipSelectedObjects(bool flipX)
        {
            foreach (var obj in Selection.gameObjects)
            {
                var flipObject = obj.GetComponent<FlipObject>();
                if (flipObject != null)
                {
                    // Record the change for undo functionality
                    Undo.RecordObject(flipObject.transform, "Flip Object");

                    if (flipX)
                    {
                        flipObject.FlipX(); // Directly call the FlipX method
                    }
                    else
                    {
                        flipObject.FlipY(); // Directly call the FlipY method
                    }

                    // Mark the object as dirty to update the scene view
                    EditorUtility.SetDirty(flipObject.transform);
                }
            }
        }
    }
}
