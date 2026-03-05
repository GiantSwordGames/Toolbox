using System;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace JamKit
{
    /// <summary>
    /// Drop this on any bone transform to get a visible gizmo + clickable handle.
    /// </summary>
    public class BoneGizmo : MonoBehaviour
    {
        [Header("Gizmo Appearance")]
        [Tooltip("Color of the bone gizmo in the Scene view.")]
        public Color gizmoColor = new Color(1f, 0.6f, 0f, 0.9f);

        public bool onlyWhenSelected = false;
        [SerializeField] private float _radius = 0.01f;
        // [SerializeField]   private bool _unparentOnStart = false;


        private void Start()
        {
            if (GetComponent<Rigidbody>())
            {
                //  transform.parent = null;
            }

        }

        public void SetupConfigurableJoint()
        {
            Rigidbody rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            rigidbody.linearDamping = 0.2f;
            // foreach (var selection in Selection.gameObjects)
            {
                Rigidbody parent = rigidbody.transform.parent.gameObject.GetOrAddComponent<Rigidbody>();
                ConfigurableJoint joint = rigidbody.gameObject.GetOrAddComponent<ConfigurableJoint>();
                joint.connectedBody = parent;
                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;

                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;

                joint.SetAngularSpringAll(3);
                joint.SetAngularDamperAll(.2f);
                joint.SetAngularLimitX(45);
                joint.SetAngularLimitY(45);
                joint.SetAngularLimitZ(45);


            }

        }
        public void SetupBoneCollider()
        {
            if (transform.childCount == 0)
                return;

            // Ensure there is a collider
            var collider = gameObject.GetOrAddComponent<CapsuleCollider>();
            collider.radius = _radius;

            // The next bone
            Transform child = transform.GetChild(0);

            // Vector from this bone to the next one
            Vector3 dir = child.position - transform.position;
            float length = dir.magnitude;

            // Height: bone length + 2x radius
            collider.height = length + _radius * 2f;

            // Capsule needs a LOCAL direction
            dir.Normalize();
            Vector3 localDir = transform.InverseTransformDirection(dir);

            // Pick the dominant axis
            // 0 = X, 1 = Y, 2 = Z
            int axis;
            Vector3 abs = new Vector3(Mathf.Abs(localDir.x), Mathf.Abs(localDir.y), Mathf.Abs(localDir.z));

            if (abs.x > abs.y && abs.x > abs.z) axis = 0;       // X axis
            else if (abs.y > abs.x && abs.y > abs.z) axis = 1;  // Y axis
            else axis = 2;                                      // Z axis

            collider.direction = axis;

            // Center midpoint between the bones (local space)
            Vector3 worldMid = transform.position + (dir * (length * 0.5f));
            collider.center = transform.InverseTransformPoint(worldMid);
        }
        private void OnDrawGizmos()
        {
            if (onlyWhenSelected) return;
            DrawGizmosInternal(isSelected: false);
            // Gizmos.DrawSphere(transform.position, 0.1f);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmosInternal(isSelected: true);
        }

        private void DrawGizmosInternal(bool isSelected)
        {
            if (enabled == false)
                return;

            var color = isSelected ? Color.yellow : gizmoColor;
            Gizmos.color = color;

            float radius = _radius;
            Gizmos.DrawSphere(transform.position, radius);

            // if (drawToChildren)
            {
                foreach (Transform child in transform)
                {
                    if (!child) continue;
                    Gizmos.DrawLine(transform.position, child.position);
                }
            }
        }

        //         private float GetWorldSpaceSize()
        //         {
        // #if UNITY_EDITOR
        //             // Try to keep a consistent on-screen size relative to Scene view camera
        //             var sceneView = UnityEditor.SceneView.currentDrawingSceneView;
        //             if (sceneView != null && sceneView.camera != null)
        //             {
        //                 float distance = Vector3.Distance(transform.position, sceneView.camera.transform.position);
        //                 // Tuned so radius feels nice with the default screenSize range
        //                 return distance * screenSize * 0.05f;
        //             }
        // #endif
        //             // Fallback if no SceneView (e.g. at runtime)
        //             return 0.02f;
        //         }
    }
}

#if UNITY_EDITOR

namespace JamKitEditor
{
    /// <summary>
    /// Custom Scene handle so you can click the bone gizmo to select it.
    /// </summary>
    [CustomEditor(typeof(JamKit.BoneGizmo))]
    [CanEditMultipleObjects]
    public class BoneGizmoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Setup Configurable Joint", GUILayout.Height(22)))
            {
                foreach (var t in targets)
                {
                    if (t is JamKit.BoneGizmo boneGizmo)
                    {
                        boneGizmo.SetupConfigurableJoint();
                    }
                }
            }
            if (GUILayout.Button("Setup Bone Collider", GUILayout.Height(22)))
            {
                foreach (var t in targets)
                {
                    if (t is JamKit.BoneGizmo boneGizmo)
                    {
                        boneGizmo.SetupBoneCollider();
                    }
                }
            }
        }

        // private void OnSceneGUI()
        // {
        //     var bone = (JamKit.BoneGizmo)target;
        //     if (bone == null || !bone.isActiveAndEnabled) return;
        //     if (!bone.clickableHandle) return;
        //
        //     Transform t = bone.transform;
        //
        //     // Handle size scales with screen distance
        //     float size = HandleUtility.GetHandleSize(t.position) * bone.handleSize;
        //
        //     Handles.color = bone.gizmoColor;
        //
        //     // Big clickable sphere that selects the bone
        //     if (Handles.Button(
        //             t.position,
        //             t.rotation,
        //             size,
        //             size,
        //             Handles.SphereHandleCap))
        //     {
        //         Selection.activeTransform = t;
        //     }
        //
        //     // Optional: also draw hierarchy lines in the handle pass so they render on top
        //     if (bone.drawToChildren)
        //     {
        //         foreach (Transform child in t)
        //         {
        //             if (!child) continue;
        //             Handles.DrawLine(t.position, child.position);
        //         }
        //     }
        // }
    }
}
#endif
