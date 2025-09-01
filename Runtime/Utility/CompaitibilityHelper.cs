using UnityEngine;
using UnityEngine.InputSystem;

namespace JamKit
{
    public static class CompaitibilityHelper
    {
        public static T FindObjectOfType<T>() where T : Object
        {
            return GameObject.FindObjectOfType<T>();
        }

        public static T[] FindObjectsByType<T>() where T : Object
        {
            return GameObject.FindObjectsOfType<T>();
        }

#if UNITY_2022
        #else
        public static bool WasPressedThisFrame(this InputAction action)
        {
            Debug.Log("Return not implented in this version of Unity");
            return false;
        }
        
        public static bool WasReleasedThisFrame(this InputAction action)
        {
            Debug.Log("Return not implented in this version of Unity");
            return false;
        }
        
        public static bool IsPressed(this InputAction action)
        {
            Debug.Log("Return not implented in this version of Unity");
            return false;
        }
#endif
        public static void SetLinearDamping(this Rigidbody rigidbody, float value)
        {
            var drag = value;
            rigidbody.drag = drag;
        }
        
        public static void SetAngularDamping(this Rigidbody rigidbody, float value)
        {
            var angularDrag = value;
            rigidbody.angularDrag = angularDrag;
        }
        
        public  static void SetLinearVelocity(this Rigidbody rigidbody, Vector3 velocity)
        {
            rigidbody.velocity = velocity;
        }
        
        public  static Vector3 GetLinearVelocity(this Rigidbody rigidbody)
        {
            return rigidbody.velocity;
        }
       
    }
}