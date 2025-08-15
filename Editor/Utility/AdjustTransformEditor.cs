using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace JamKit
{
    [CustomEditor(typeof(AdjustTransform))]
    public class AdjustTransformEditor : UnityEditor.Editor
    {
        private Vector3 position;
        
        private void Awake()
        {
            AdjustTransform transform = target as AdjustTransform;
            position = transform.transform.position;
        }

        private void OnSceneGUI()
        {
            AdjustTransform adjustTransform = target as AdjustTransform;
            Transform transform = adjustTransform.transform;

            
            EditorGUI.BeginChangeCheck();
            position = Handles.PositionHandle(position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                List<Vector3> childPositions = new List<Vector3>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    childPositions.Add(transform.GetChild(i).position);

                }
               
                // Undo.RecordObject(transform, "Change Look At Target Position");
                // example.targetPosition = newTargetPosition;
                // example.Update();

                transform.position = position;

                for (int i = 0; i < childPositions.Count; i++)
                {
                    transform.GetChild(i).position = childPositions[i];
                }
                
            }
        }
    }
}