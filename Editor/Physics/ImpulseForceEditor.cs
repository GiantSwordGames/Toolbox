using UnityEditor;
using UnityEngine;

namespace GiantSword
{
  
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ImpulseForce))]
    public class ImpulseForceEditor : CustomEditorBase<ImpulseForce>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
    
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Apply Impulse"))
            {
                
                foreach (ImpulseForce addForce in targetObjects)
                {
                    addForce.Trigger();
                }
            }
        }
    }
}
