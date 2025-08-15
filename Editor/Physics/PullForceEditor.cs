namespace JamKit
{
    using UnityEditor;
    using UnityEngine;
    
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PullForce))]
    public class PullForceEditor : CustomEditorBase<PullForce>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
    
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Apply Impulse"))
            {
                
                foreach (PullForce addForce in targetObjects)
                {
                    addForce.Pull();
                }
            }
        }
    }
}
