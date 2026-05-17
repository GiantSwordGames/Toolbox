using JamKit;
using UnityEngine;
using UnityEditor;

namespace GiantSword
{
    public class RotateYStep : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float _stepDegrees = 90f;

        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmo = true;
        [SerializeField] private float _gizmoLength = 0.5f;

        public float stepDegrees => _stepDegrees;

        public void RotatePositive() => ApplyRotation(+1);
        public void RotateNegative() => ApplyRotation(-1);

        /// <summary>
        /// direction should be +1 or -1. Only rotates around Y.
        /// </summary>
        public void ApplyRotation(int direction)
        {
            if (direction == 0) return;

            RuntimeEditorHelper.RecordObjectUndo(transform, "Rotate Y By Step");

            Vector3 e = transform.eulerAngles;
            e.y = NormalizeAngle(e.y + direction * _stepDegrees);
            transform.eulerAngles = e;
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RotateYStep))]
    [CanEditMultipleObjects]
    public class RotateYStepEditor : Editor
    {
        private SerializedProperty _stepProp;
        private SerializedProperty _drawGizmoProp;
        private SerializedProperty _gizmoLengthProp;
        private static  Preference<bool> _advancedFoldout = new Preference<bool>("rotationadvanced", false);
        private static  Preference<bool> infoFoldout = new Preference<bool>("rotationInfoFoldout", false);

        private void OnEnable()
        {
            _stepProp        = serializedObject.FindProperty("_stepDegrees");
            _drawGizmoProp   = serializedObject.FindProperty("_drawGizmo");
            _gizmoLengthProp = serializedObject.FindProperty("_gizmoLength");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(" -90", GUILayout.Height(24)))
            {
                RotateSelected(-1);
            }

            if (GUILayout.Button(" +90", GUILayout.Height(24)))
            {
                RotateSelected(+1);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            _advancedFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(_advancedFoldout.value, "Advanced");

            if (_advancedFoldout)
            {
                EditorGUILayout.Space(6);

                EditorGUILayout.LabelField("Rotation Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_stepProp, new GUIContent("Step Degrees"));

                EditorGUILayout.Space(6);

                EditorGUILayout.LabelField("Gizmos", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_drawGizmoProp, new GUIContent("Draw Direction Gizmo"));
                if (_drawGizmoProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_gizmoLengthProp, new GUIContent("Gizmo Length"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(8);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            infoFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(infoFoldout.value, "Info");
            if (infoFoldout)
            {
                EditorGUILayout.HelpBox(
                    "Rotate Y Step\n\n" +
                    "• Attach to any object you want to rotate.\n" +
                    "• In the Scene View, press ',' or '.' to rotate all selected\n" +
                    "  objects with this component by -step / +step around Y.\n" +
                    "• Use the buttons below to rotate from the Inspector.\n",
                    MessageType.Info
                );
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void RotateSelected(int direction)
        {
            var objs = targets;
            Undo.SetCurrentGroupName("Rotate Y By Step (Inspector)");
            int group = Undo.GetCurrentGroup();

            foreach (var o in objs)
            {
                var comp = o as RotateYStep;
                if (comp == null) continue;

                comp.ApplyRotation(direction);
            }

            Undo.CollapseUndoOperations(group);
            SceneView.RepaintAll();
        }
    }

    [InitializeOnLoad]
    public static class RotateYStepHotkeys
    {
        static RotateYStepHotkeys()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e == null || e.type != EventType.KeyDown)
                return;

            int direction = 0;

            // Physical keys: , and .  (often < and > with Shift)
            if (e.keyCode == KeyCode.Comma)
                direction = -1;
            else if (e.keyCode == KeyCode.Period)
                direction = +1;

            if (direction == 0)
                return;

            var selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0)
                return;

            bool anyRotatable = false;
            foreach (var go in selected)
            {
                if (go != null && go.GetComponent<RotateYStep>() != null)
                {
                    anyRotatable = true;
                    break;
                }
            }
            if (!anyRotatable)
                return;

            Undo.SetCurrentGroupName("Rotate Y By Step (Hotkey)");
            int group = Undo.GetCurrentGroup();

            foreach (var go in selected)
            {
                if (go == null) continue;
                var comp = go.GetComponent<RotateYStep>();
                if (comp == null) continue;

                comp.ApplyRotation(direction);
            }

            Undo.CollapseUndoOperations(group);

            e.Use();
            SceneView.RepaintAll();
        }
    }
#endif
}
