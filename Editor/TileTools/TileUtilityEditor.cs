using UnityEditor;
using UnityEngine;

namespace Meat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TileUtility))]
    public class TileUtilityEditor : Editor
    {
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI; // Subscribe to SceneView event
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI; // Unsubscribe to prevent memory leaks
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Label("Toggle Visibility with Arrow Keys and <> in Scene View");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Floor <"))
            {
                foreach (TileUtility tileUtility in targets)
                {
                    Undo.RecordObject(tileUtility.floor, "Toggle Floor");
                    ToggleGameObject(tileUtility.floor);
                }
            }
            if (GUILayout.Button("North ↑"))
            {
                foreach (TileUtility tileUtility in targets)
                {
                    Undo.RecordObject(tileUtility.wallNorth, "Toggle Wall North");
                    ToggleGameObject(tileUtility.wallNorth);
                }
            }
            if (GUILayout.Button("West ←"))
            {
                foreach (TileUtility tileUtility in targets)
                {
                    Undo.RecordObject(tileUtility.wallWest, "Toggle Wall West");
                    ToggleGameObject(tileUtility.wallWest);
                }
            }
            if (GUILayout.Button("South ↓"))
            {
                foreach (TileUtility tileUtility in targets)
                {
                    Undo.RecordObject(tileUtility.wallSouth, "Toggle Wall South");
                    ToggleGameObject(tileUtility.wallSouth);
                }
            }
            if (GUILayout.Button("East →"))
            {
                foreach (TileUtility tileUtility in targets)
                {
                    Undo.RecordObject(tileUtility.wallEast, "Toggle Wall East");
                    ToggleGameObject(tileUtility.wallEast);
                }
            }
            if (GUILayout.Button("Ceiling >"))
            {
                foreach (TileUtility tileUtility in targets)
                {
                    Undo.RecordObject(tileUtility.ceiling, "Toggle Ceiling");
                    ToggleGameObject(tileUtility.ceiling);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10); // Add some space before the clear button

            // Add the Clear button to clear only the walls
            if (GUILayout.Button("Clear Walls"))
            {
                foreach (TileUtility tileUtility in targets)
                {
                    Undo.RecordObject(tileUtility, "Clear Walls");
                    ToggleGameObject(tileUtility.wallNorth, false);
                    ToggleGameObject(tileUtility.wallWest, false);
                    ToggleGameObject(tileUtility.wallSouth, false);
                    ToggleGameObject(tileUtility.wallEast, false);
                }
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                bool keyHandled = false; // Track if the key was handled

                foreach (TileUtility tileUtility in targets)
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.UpArrow:
                            Undo.RecordObject(tileUtility.wallNorth, "Toggle Wall North");
                            ToggleGameObject(tileUtility.wallNorth);
                            keyHandled = true;
                            break;
                        case KeyCode.LeftArrow:
                            Undo.RecordObject(tileUtility.wallWest, "Toggle Wall West");
                            ToggleGameObject(tileUtility.wallWest);
                            keyHandled = true;
                            break;
                        case KeyCode.DownArrow:
                            Undo.RecordObject(tileUtility.wallSouth, "Toggle Wall South");
                            ToggleGameObject(tileUtility.wallSouth);
                            keyHandled = true;
                            break;
                        case KeyCode.RightArrow:
                            Undo.RecordObject(tileUtility.wallEast, "Toggle Wall East");
                            ToggleGameObject(tileUtility.wallEast);
                            keyHandled = true;
                            break;
                        case KeyCode.Comma:  // For the '<' key
                            Undo.RecordObject(tileUtility.floor, "Toggle Floor");
                            ToggleGameObject(tileUtility.floor);
                            keyHandled = true;
                            break;
                        case KeyCode.Period: // For the '>' key
                            Undo.RecordObject(tileUtility.ceiling, "Toggle Ceiling");
                            ToggleGameObject(tileUtility.ceiling);
                            keyHandled = true;
                            break;
                    }

                    if (keyHandled)
                    {
                        // Mark the scene as dirty to enable undo and save
                        EditorUtility.SetDirty(tileUtility);
                    }
                }

                if (keyHandled)
                {
                    e.Use(); // Consume the event only if a relevant key was pressed
                }
            }
        }

        // Updated ToggleGameObject method to optionally take a target state
        private void ToggleGameObject(GameObject obj, bool? state = null)
        {
            if (obj != null)
            {
                bool newState = state ?? !obj.activeSelf;
                obj.SetActive(newState);
            }
        }
    }
}
