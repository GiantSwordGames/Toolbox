using UnityEngine;

namespace JamKit
{
    public static class EditorInput
    {
        static bool AVAILABLE_IN_BUILD = true;
        public static bool GetKeyDown(KeyCode code)
        {
            return (Input.GetKeyDown(code) && (AVAILABLE_IN_BUILD || Application.isEditor));
        }

        public static bool GetKey(KeyCode code)
        {
            return (Input.GetKey(code) &&  (AVAILABLE_IN_BUILD || Application.isEditor));
        }
        
        public static bool GetKeysDown(KeyCode code, KeyCode modifier)
        {
            return (Input.GetKeyDown(code) && Input.GetKey(modifier) && (AVAILABLE_IN_BUILD || Application.isEditor));
        }
        
        public static bool ShiftModifier(KeyCode code)
        {
            return (Input.GetKeyDown(code) && Input.GetKey(KeyCode.LeftShift) && (AVAILABLE_IN_BUILD || Application.isEditor));
        }
    }
}