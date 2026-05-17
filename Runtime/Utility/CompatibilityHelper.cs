using UnityEngine;
using UnityEngine.InputSystem;

namespace JamKit
{
    public static class CompatibilityHelper
    {
        public static T FindObjectOfType<T>() where T : Object
        {
#if UNITY_6000
            return GameObject.FindAnyObjectByType<T>(FindObjectsInactive.Include);
#else            
            return GameObject.FindObjectOfType<T>();
#endif
        }      
        
        
        public static T[] FindObjectsOfType<T>() where T : Object
        {
            return FindObjectsByType<T>();
        }

        public static T[] FindObjectsByType<T>() where T : Object
        {
#if UNITY_6000
            return GameObject.FindObjectsByType<T>();
#else
            return GameObject.FindObjectsOfType<T>();
#endif

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
            rigidbody.linearDamping = drag;
        }
        
        public static void SetAngularDamping(this Rigidbody rigidbody, float value)
        {
            var angularDrag = value;
            rigidbody.angularDamping = angularDrag;
        }
        
        public  static void SetLinearVelocity(this Rigidbody rigidbody, Vector3 velocity)
        {
            rigidbody.linearVelocity = velocity;
        }
        
        public  static Vector3 GetLinearVelocity(this Rigidbody rigidbody)
        {
            return rigidbody.linearVelocity;
        }
       
    }
}