using UnityEngine;
using UnityEditor;

namespace GiantSword
{
    public class CheckIfSnapped : MonoBehaviour
    {
        public enum PositionSpace
        {
            World,
            Local
        }

        [SerializeField] private Vector3 snapInterval = Vector3.one;
        [SerializeField] private PositionSpace positionSpace = PositionSpace.World;

        public Vector3 SnapInterval => snapInterval;
        public PositionSpace Space => positionSpace;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CheckIfSnapped))]
    [CanEditMultipleObjects]
    public class CheckIfSnappedEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Snap Selected"))
            {
                foreach (var t in targets)
                {
                    var comp = (CheckIfSnapped)t;
                    ApplySnap(comp.gameObject, comp.SnapInterval, comp.Space);
                }

                // Refresh hierarchy icons
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private void ApplySnap(GameObject go, Vector3 interval, CheckIfSnapped.PositionSpace space)
        {
            Undo.RecordObject(go.transform, "Snap Transform");

            if (space == CheckIfSnapped.PositionSpace.World)
            {
                // Snap world position
                Vector3 pos = go.transform.position;
                pos.x = Mathf.Round(pos.x / interval.x) * interval.x;
                pos.y = Mathf.Round(pos.y / interval.y) * interval.y;
                pos.z = Mathf.Round(pos.z / interval.z) * interval.z;
                go.transform.position = pos;
            }
            else
            {
                // Snap local position
                Vector3 local = go.transform.localPosition;
                local.x = Mathf.Round(local.x / interval.x) * interval.x;
                local.y = Mathf.Round(local.y / interval.y) * interval.y;
                local.z = Mathf.Round(local.z / interval.z) * interval.z;
                go.transform.localPosition = local;
            }

            // Snap rotation (always local euler)
            Vector3 rot = go.transform.eulerAngles;
            rot.x = Mathf.Round(rot.x / interval.x) * interval.x;
            rot.y = Mathf.Round(rot.y / interval.y) * interval.y;
            rot.z = Mathf.Round(rot.z / interval.z) * interval.z;
            go.transform.eulerAngles = rot;
        }
    }

    [InitializeOnLoad]
    public static class CheckIfSnappedHierarchyDrawer
    {
        static GUIContent snapIcon;

        static CheckIfSnappedHierarchyDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

            snapIcon = EditorGUIUtility.IconContent("SceneViewSnap-Off");
            snapIcon.tooltip = "This object is not snapped to its interval";
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            var snapComp = go.GetComponent<CheckIfSnapped>();
            if (snapComp == null) return;

            // Pick local or world
            Vector3 pos = (snapComp.Space == CheckIfSnapped.PositionSpace.World)
                ? go.transform.position
                : go.transform.localPosition;

            if (!IsSnapped(pos, snapComp.SnapInterval) ||
                !IsSnapped(go.transform.eulerAngles, snapComp.SnapInterval))
            {
                Rect r = new Rect(selectionRect);
                r.x = r.xMax - 18;
                r.width = 16;
                GUI.Label(r, snapIcon);
            }
        }

        private static bool IsSnapped(Vector3 v, Vector3 interval)
        {
            return IsSnapped(v.x, interval.x) &&
                   IsSnapped(v.y, interval.y) &&
                   IsSnapped(v.z, interval.z);
        }

        private static bool IsSnapped(float value, float step)
        {
            if (Mathf.Approximately(step, 0f)) return true;
            float snapped = Mathf.Round(value / step) * step;
            return Mathf.Approximately(value, snapped);
        }
    }
#endif
}
