using UnityEditor;

namespace JamKit
{
    [CustomEditor(typeof(AlwaysRedrawTheInspector))]
    public class AlwaysRedrawTheInspectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Repaint();
        }
    }
}