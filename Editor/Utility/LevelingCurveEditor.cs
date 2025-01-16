using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    [CustomEditor(typeof(LevellingCurve))]
    public class LevelingCurveEditor : CustomEditorBase<LevellingCurve>
    {
        private Vector2 _scroll;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Label("");

            List<(int, float)> samples = new List<(int, float)>();
            for (float i = targetObject.range.min; i <= targetObject.range.max; i++)
            {
                (int i, float) valueTuple = ((int)i, targetObject.EvaluateLevel(i));
                samples.Add(valueTuple);
            }

            // Plotting the curve
            if (samples.Count > 1)
            {
                Rect curveRect = GUILayoutUtility.GetRect(10, 200); // Adjust height as needed
                EditorGUI.DrawRect(curveRect, new Color(0.15f, 0.15f, 0.15f)); // Background color

                Handles.color = Color.green;
                float maxValue = Mathf.Max(targetObject.EvaluateLevel(targetObject.range.min), targetObject.EvaluateLevel(targetObject.range.max));
                float minValue = Mathf.Min(targetObject.EvaluateLevel(targetObject.range.min), targetObject.EvaluateLevel(targetObject.range.max));

                for (int i = 0; i < samples.Count - 1; i++)
                {
                    Vector2 pointA = new Vector2(
                        Mathf.Lerp(curveRect.x, curveRect.xMax, (float)(samples[i].Item1 - targetObject.range.min) / (targetObject.range.max - targetObject.range.min)),
                        Mathf.Lerp(curveRect.yMax, curveRect.y, samples[i].Item2 / maxValue)
                    );

                    Vector2 pointB = new Vector2(
                        Mathf.Lerp(curveRect.x, curveRect.xMax, (float)(samples[i + 1].Item1 - targetObject.range.min) / (targetObject.range.max - targetObject.range.min)),
                        Mathf.Lerp(curveRect.yMax, curveRect.y, samples[i + 1].Item2 / maxValue)
                    );

                    Handles.DrawLine(pointA, pointB);
                }

                // Display the range as a legend at the bottom
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Min X: {targetObject.range.min:F2}", GUILayout.Width(100));
                GUILayout.Label($"Max X: {targetObject.range.max:F2}", GUILayout.Width(100));
                GUILayout.Label($"Min Y: {minValue:F2}", GUILayout.Width(100));
                GUILayout.Label($"Max Y: {maxValue:F2}", GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
            GUILayout.Label("");

            _scroll =  GUILayout.BeginScrollView(_scroll, GUILayout.Height(300));

            // Display the list of values as labels
            foreach (var sample in samples)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(sample.Item1.ToString());
                GUILayout.Label(sample.Item2.ToString());
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndScrollView();
        }
    }
}
