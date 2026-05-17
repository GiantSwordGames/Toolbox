using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace JamKit
{
    public class TileDrawer : MonoBehaviour
    {
        public enum PositionSpace
        {
            World,
            Local
        }

        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private Vector3 _tileSize = Vector3.one;
        [SerializeField] private bool _alwaysDrawPivot = false;
        [SerializeField] private PositionSpace _positionSpace = PositionSpace.World;
        [SerializeField] private bool _autoCalculateBounds = true;

        public Vector3 tileSize
        {
            get
            {
                Vector3 size = _tileSize;

                if (autoCalculateBounds)
                {
#if UNITY_EDITOR
                    TileDrawingTool.TryGetPrefabBounds(this, out Bounds autoCalculatedBounds);
                    size = autoCalculatedBounds.size;
#endif
                }

                size.x = Mathf.Max(size.x, Mathf.Epsilon);
                size.y = Mathf.Max(size.y, Mathf.Epsilon);
                size.z = Mathf.Max(size.z, Mathf.Epsilon);
                return size;
            }
        }

        public Vector3 snapInterval => _tileSize;
        public PositionSpace positionSpace => _positionSpace;
        public Vector3 offset => _offset;

        public bool autoCalculateBounds
        {
            get => _autoCalculateBounds;
            set => _autoCalculateBounds = value;
        }

        private void OnDrawGizmos()
        {
            if (_alwaysDrawPivot)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(transform.position + _offset, 1f);
            }
        }

        public void RotateAroundCenter(float y)
        {
            RuntimeEditorHelper.RecordObjectUndo(transform, "Rotate ");
            transform.RotateAround(transform.TransformPoint(offset), Vector3.up, y);
        }

        public void ResetRotation()
        {
            RuntimeEditorHelper.RecordObjectUndo(transform, "Rotate ");
            transform.RotateAround(
                transform.TransformPoint(offset),
                transform.localRotation.eulerAngles.normalized,
                -transform.localRotation.eulerAngles.magnitude);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Toggles editor visibility for this object or all matching prefab instances.
        /// </summary>
        public void ToggleEditorVisibility()
        {
            GameObject[] targets = GetVisibilityTargets();

            if (targets == null || targets.Length == 0)
            {
                targets = new[] { gameObject };
            }

            SceneVisibilityManager visibilityManager = SceneVisibilityManager.instance;

            bool anyVisible = false;

            for (int i = 0; i < targets.Length; i++)
            {
                GameObject target = targets[i];

                if (target == null)
                    continue;

                if (!visibilityManager.IsHidden(target, true))
                {
                    anyVisible = true;
                    break;
                }
            }

            bool shouldHide = anyVisible;

            Undo.RecordObjects(targets, "Toggle Editor Visibility");

            for (int i = 0; i < targets.Length; i++)
            {
                GameObject target = targets[i];

                if (target == null)
                    continue;

                if (shouldHide)
                {
                    visibilityManager.Hide(target, true);
                }
                else
                {
                    visibilityManager.Show(target, true);
                }

                EditorUtility.SetDirty(target);
            }

            SceneView.RepaintAll();
            EditorApplication.RepaintHierarchyWindow();
        }

        /// <summary>
        /// Solos this prefab family in editor scene visibility.
        ///
        /// IMPORTANT:
        /// This intentionally reuses GetVisibilityTargets(), because that logic
        /// already correctly finds matching prefab instances for the normal
        /// visibility toggle. Solo simply inverts that result:
        ///
        /// - show the matching targets
        /// - hide everything else
        ///
        /// This uses SceneVisibilityManager only. It does not deactivate objects.
        /// </summary>
        public void ToggleSoloSamePrefab()
        {
            SceneVisibilityManager visibilityManager = SceneVisibilityManager.instance;
            Object soloIdentity = GetSoloIdentity();

            if (TileDrawerEditor.IsSoloActiveFor(soloIdentity))
            {
                visibilityManager.ShowAll();
                TileDrawerEditor.ClearSoloState();
                SceneView.RepaintAll();
                EditorApplication.RepaintHierarchyWindow();
                return;
            }

            GameObject[] visibleTargets = GetVisibilityTargets();

            if (visibleTargets == null || visibleTargets.Length == 0)
            {
                visibleTargets = new[] { gameObject };
            }

            HashSet<GameObject> visibleSet = new HashSet<GameObject>(visibleTargets);

            visibilityManager.ShowAll();

            GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            for (int i = 0; i < allGameObjects.Length; i++)
            {
                GameObject candidate = allGameObjects[i];

                if (ShouldIgnoreForSolo(candidate))
                    continue;

                if (visibleSet.Contains(candidate))
                    continue;

                visibilityManager.Hide(candidate, true);
            }

            // Explicitly reshow the matching objects in case any were hidden before.
            foreach (GameObject target in visibleTargets)
            {
                if (target == null)
                    continue;

                visibilityManager.Show(target, true);
            }

            TileDrawerEditor.SetSoloState(soloIdentity);

            SceneView.RepaintAll();
            EditorApplication.RepaintHierarchyWindow();
        }

        private GameObject[] GetVisibilityTargets()
        {
            if (!PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                return new[] { gameObject };
            }

            GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefabSource == null)
            {
                return new[] { gameObject };
            }

            List<GameObject> matches = new List<GameObject>();
            GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            for (int i = 0; i < allGameObjects.Length; i++)
            {
                GameObject candidate = allGameObjects[i];

                if (candidate == null)
                    continue;

                if (!candidate.scene.IsValid())
                    continue;

                if (EditorUtility.IsPersistent(candidate))
                    continue;

                if (PrefabUtility.GetPrefabInstanceStatus(candidate) == PrefabInstanceStatus.NotAPrefab)
                    continue;

                GameObject candidateSource = PrefabUtility.GetCorrespondingObjectFromSource(candidate);
                if (candidateSource != prefabSource)
                    continue;

                matches.Add(candidate);
            }

            return matches.ToArray();
        }

        /// <summary>
        /// Returns the identity used by the editor to know whether the current
        /// solo mode corresponds to this drawer.
        ///
        /// This intentionally follows the same target-finding logic used by
        /// GetVisibilityTargets().
        /// </summary>
        private Object GetSoloIdentity()
        {
            GameObject[] targets = GetVisibilityTargets();

            if (targets != null && targets.Length > 0)
            {
                GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                if (prefabSource != null)
                    return prefabSource;
            }

            return gameObject;
        }

        /// <summary>
        /// Filters out invalid scene objects, assets, and hidden editor-only objects.
        /// </summary>
        private static bool ShouldIgnoreForSolo(GameObject candidate)
        {
            if (candidate == null)
                return true;

            if (!candidate.scene.IsValid())
                return true;

            if (EditorUtility.IsPersistent(candidate))
                return true;

            if ((candidate.hideFlags & HideFlags.HideInHierarchy) != 0)
                return true;

            return false;
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TileDrawer))]
    [CanEditMultipleObjects]
    public class TileDrawerEditor : Editor
    {
        private SerializedProperty _offsetProp;
        private SerializedProperty _tileSizeProp;
        private SerializedProperty _positionSpaceProp;
        private SerializedProperty _drawPivotProp;
        private SerializedProperty _autoCalculateBounds;

        private static Preference<bool> _snapFoldout = new Preference<bool>("tileDrawerSnapFoldout", true);
        private static Preference<bool> _drawingFoldout = new Preference<bool>("tileDrawerDrawingFoldout", false);

        /// <summary>
        /// Shared solo state across TileDrawer inspectors.
        /// </summary>
        private static bool _soloModeActive;
        private static Object _soloIdentity;

        private void OnEnable()
        {
            _offsetProp = serializedObject.FindProperty("_offset");
            _drawPivotProp = serializedObject.FindProperty("_alwaysDrawPivot");
            _tileSizeProp = serializedObject.FindProperty("_tileSize");
            _positionSpaceProp = serializedObject.FindProperty("_positionSpace");
            _autoCalculateBounds = serializedObject.FindProperty("_autoCalculateBounds");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("Snap To Grid", GUILayout.Height(22)))
            {
                foreach (var t in targets)
                {
                    var comp = t as TileDrawer;
                    if (comp != null)
                    {
                        ApplySnap(comp.gameObject, comp.snapInterval, comp.positionSpace);
                    }
                }

                EditorApplication.RepaintHierarchyWindow();
            }

            if (GUILayout.Button("Toggle Prefab Visibility", GUILayout.Height(22)))
            {
                foreach (var t in targets)
                {
                    TileDrawer drawer = t as TileDrawer;
                    if (drawer != null)
                    {
                        drawer.ToggleEditorVisibility();
                    }
                }
            }

            if (targets.Length == 1)
            {
                TileDrawer drawer = target as TileDrawer;
                if (drawer != null)
                {
                    string soloLabel = IsSoloActiveFor(drawer)
                        ? "Unsolo Same Prefab"
                        : "Solo Same Prefab";

                    if (GUILayout.Button(soloLabel, GUILayout.Height(22)))
                    {
                        drawer.ToggleSoloSamePrefab();
                    }
                }
            }

            EditorGUILayout.Space(4);

            _drawingFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(_drawingFoldout.value, "Advanced");

            if (_drawingFoldout)
            {
                EditorGUILayout.PropertyField(_positionSpaceProp, new GUIContent("Position Space"));
                EditorGUILayout.Space(5);

                if (_autoCalculateBounds.boolValue == false)
                {
                    EditorGUILayout.PropertyField(_tileSizeProp);
                }
                else
                {
                    EditorGUILayout.LabelField("Tile Size", "Auto Calculated");
                }

                EditorGUILayout.PropertyField(_offsetProp);
                EditorGUILayout.PropertyField(_drawPivotProp);
                EditorGUILayout.PropertyField(_autoCalculateBounds);
                TileDrawingTool.enableTileDrawing.DrawDefaultGUI();
                TileDrawingTool.enablePrefabCycling.DrawDefaultGUI();

                EditorGUILayout.Space(5);

                if (GUILayout.Button("Reset Rotation Around Pivot", GUILayout.Height(22)))
                {
                    foreach (var target in targets)
                    {
                        ((TileDrawer)target).ResetRotation();
                    }
                }

                if (GUILayout.Button("Randomize Rotation", GUILayout.Height(22)))
                {
                    foreach (var target in targets)
                    {
                        ((TileDrawer)target).RotateAroundCenter(Random.Range(0, 4) * 90);
                    }
                }

                if (GUILayout.Button("Calculate Tile Size From Prefab", GUILayout.Height(22)))
                {
                    foreach (Object t in targets)
                    {
                        CalculateTileSize((TileDrawer)t);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            _snapFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(_snapFoldout.value, "How To Use");
            if (_snapFoldout)
            {
                EditorGUILayout.HelpBox(
                    "Tile Drawer Tool\n\n" +
                    "• Hold CTRL in Scene View to activate the drawing tool.\n" +
                    "• Click + drag to place tiles.\n" +
                    "• Hold SHIFT + CTRL to erase instead.\n" +
                    "• Tile size defines both placement spacing and snapping interval.\n" +
                    "• Toggle Editor Visibility hides/shows matching prefab instances.\n" +
                    "• Solo Same Prefab hides everything except the objects returned by the existing matching logic.\n" +
                    "• This component also checks if the object is snapped to its grid and shows an icon in the Hierarchy.",
                    MessageType.Info
                );
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        public static bool IsSoloActiveFor(TileDrawer drawer)
        {
            if (drawer == null)
                return false;

            Object identity = GetSoloIdentity(drawer);
            return IsSoloActiveFor(identity);
        }

        public static bool IsSoloActiveFor(Object identity)
        {
            return _soloModeActive && _soloIdentity == identity;
        }

        public static void SetSoloState(Object identity)
        {
            _soloModeActive = true;
            _soloIdentity = identity;
        }

        public static void ClearSoloState()
        {
            _soloModeActive = false;
            _soloIdentity = null;
        }

        private static Object GetSoloIdentity(TileDrawer drawer)
        {
            if (drawer == null)
                return null;

            if (PrefabUtility.IsPartOfPrefabInstance(drawer.gameObject))
            {
                GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(drawer.gameObject);
                if (prefabSource != null)
                    return prefabSource;
            }

            return drawer.gameObject;
        }

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