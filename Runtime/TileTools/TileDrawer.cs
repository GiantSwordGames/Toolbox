using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace JamKit
{
    public class TileDrawer : MonoBehaviour
    {
        public enum PositionSpace
        {
            World,
            Local
        }
        public static Preference<bool> DrawPivots = new Preference<bool>("drawTileDrawerPivots", true);

        [Header("Tile Drawer")]
        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private Vector3 _tileSize = Vector3.one;

        [Header("Snap / Debug")]
        [SerializeField] private PositionSpace _positionSpace = PositionSpace.World;

        [SerializeField] private bool _autoCalculateBounds = true;

        public Vector3 tileSize => _tileSize;

        // Reuse tile size as snap interval
        public Vector3 snapInterval => _tileSize;
        public PositionSpace positionSpace => _positionSpace;

        public Vector3 offset
        {
            get => _offset;
            set => _offset = value;
        }

        public bool autoCalculateBounds
        {
            get => _autoCalculateBounds;
            set => _autoCalculateBounds = value;
        }

        private void OnDrawGizmosSelected()
        {
            if (DrawPivots)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position + _offset, 0.05f);
            }
        }
    }

#if UNITY_EDITOR
      [CustomEditor(typeof(TileDrawer))]
    [CanEditMultipleObjects]
    public class TileDrawerEditor : Editor
    {
        private SerializedProperty _offsetProp;
        private SerializedProperty _tileSizeProp;
        private SerializedProperty _positionSpaceProp;
        private SerializedProperty _autoCalculateBounds;

        private static Preference<bool>  _snapFoldout = new Preference<bool>("tileDrawerSnapFoldout", true);
        private static  Preference<bool> _drawingFoldout = new Preference<bool>("tileDrawerDrawingFoldout", false);

        private void OnEnable()
        {
            _offsetProp = serializedObject.FindProperty("_offset");
            _tileSizeProp = serializedObject.FindProperty("_tileSize");
            _positionSpaceProp = serializedObject.FindProperty("_positionSpace");
            _autoCalculateBounds = serializedObject.FindProperty("_autoCalculateBounds");
            
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            
        
            if (GUILayout.Button("Snap Selected To Grid", GUILayout.Height(22)))
            {
                foreach (var t in targets)
                {
                    var comp = t as TileDrawer;
                    ApplySnap(comp.gameObject, comp.snapInterval, comp.positionSpace);
                }

                EditorApplication.RepaintHierarchyWindow();
            }
            EditorGUILayout.Space(4);

            _drawingFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(_drawingFoldout.value, "Advanced");

    

            // ----------- EDITOR DRAWING TOOL -----------
            if (_drawingFoldout)
            {

                EditorGUILayout.PropertyField(_offsetProp);
                EditorGUILayout.PropertyField(_tileSizeProp);
                EditorGUILayout.PropertyField(_autoCalculateBounds, new GUIContent("Auto Calculate Tile Size"));

                EditorGUILayout.Space(2);

                if (GUILayout.Button("Calculate Tile Size From Prefab", GUILayout.Height(22)))
                {
                    foreach (Object t in targets)
                        CalculateTileSize((TileDrawer)t);
                }
                if (GUILayout.Button(TileDrawingTool.disableTileDrawingTool.value
                        ? "Enable Tile Drawing Tool"
                        : "Disable Tile Drawing Tool", GUILayout.Height(24)))
                {
                    TileDrawingTool.disableTileDrawingTool.value =
                        !TileDrawingTool.disableTileDrawingTool.value;
                }

            EditorGUILayout.Space(8);

         
                EditorGUILayout.PropertyField(_positionSpaceProp, new GUIContent("Position Space"));
                TileDrawer.DrawPivots.DrawDefaultGUI();

               
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            // ----------- SNAPPING SETTINGS -----------
            _snapFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(_snapFoldout.value, "How To Use");
            if (_snapFoldout)
            {
                // ----------- HELP BOX -----------
                EditorGUILayout.HelpBox(
                    "Tile Drawer Tool\n\n" +
                    "• Hold CTRL in Scene View to activate the drawing tool.\n" +
                    "• Click + drag to place tiles.\n" +
                    "• Hold SHIFT + CTRL to erase instead.\n" +
                    "• Tile size defines both placement spacing and snapping interval.\n" +
                    "• This component also checks if the object is snapped to its grid and shows an icon in the Hierarchy.",
                    MessageType.Info
                );
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }


        // ------------------------------------------------
        //  SNAP LOGIC
        // ------------------------------------------------
        private void ApplySnap(GameObject go, Vector3 interval, TileDrawer.PositionSpace space)
        {
            Transform tr = go.transform;
            Undo.RecordObject(tr, "Snap Transform");

            if (space == TileDrawer.PositionSpace.World)
            {
                Vector3 p = tr.position;
                p.x = Mathf.Round(p.x / interval.x) * interval.x;
                p.y = Mathf.Round(p.y / interval.y) * interval.y;
                p.z = Mathf.Round(p.z / interval.z) * interval.z;
                tr.position = p;
            }
            else
            {
                Vector3 p = tr.localPosition;
                p.x = Mathf.Round(p.x / interval.x) * interval.x;
                p.y = Mathf.Round(p.y / interval.y) * interval.y;
                p.z = Mathf.Round(p.z / interval.z) * interval.z;
                tr.localPosition = p;
            }

            Vector3 r = tr.eulerAngles;
            r.x = Mathf.Round(r.x / interval.x) * interval.x;
            r.y = Mathf.Round(r.y / interval.y) * interval.y;
            r.z = Mathf.Round(r.z / interval.z) * interval.z;
            tr.eulerAngles = r;
        }


        // ------------------------------------------------
        //  AUTO TILE SIZE LOGIC
        // ------------------------------------------------
        private void CalculateTileSize(TileDrawer drawer)
        {
            GameObject prefab = drawer.gameObject;
            var meshRenderers = prefab.GetComponentsInChildren<MeshRenderer>();
            var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();

            if (meshRenderers.Length == 0 && spriteRenderers.Length == 0)
            {
                Debug.LogWarning("TileDrawer: No MeshRenderer or SpriteRenderer found.");
                return;
            }

            Bounds bounds = new Bounds(prefab.transform.position, Vector3.zero);
            foreach (var mr in meshRenderers) bounds.Encapsulate(mr.bounds);
            foreach (var sr in spriteRenderers) bounds.Encapsulate(sr.bounds);

            Vector3 size = bounds.size;
            Vector3 rounded = new Vector3(
                Mathf.Round(size.x * 100f) / 100f,
                Mathf.Round(size.y * 100f) / 100f,
                Mathf.Round(size.z * 100f) / 100f
            );

            Undo.RecordObject(drawer, "Auto-calc Tile Size");
            drawer.GetType()
                .GetField("_tileSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(drawer, rounded);

            EditorUtility.SetDirty(drawer);
            Debug.Log($"[TileDrawer] Tile size updated to {rounded}");
            
          
        }
    }
#endif
}
