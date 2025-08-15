
    using UnityEditor;
    using UnityEngine;

    namespace JamKit
    {
        [CanEditMultipleObjects]
        [CustomEditor(typeof(PokeForce))]
        public class PokeForceEditor : CustomEditorBase<PokeForce>
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUI.enabled = Application.isPlaying;
                if (GUILayout.Button("Apply Impulse"))
                {

                    foreach (PokeForce addForce in targetObjects)
                    {
                        addForce.Poke();
                    }
                }
            }
        }
    }
