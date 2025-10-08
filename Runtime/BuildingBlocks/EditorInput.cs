using UnityEngine;

namespace JamKit
{
    public static class EditorInput
    {
        static bool AVAILABLE_IN_BUILD = true;
        public static bool IsKeyDown(KeyCode code)
        {
            return (Input.GetKeyDown(code) && (AVAILABLE_IN_BUILD || Application.isEditor));
        }

        public static bool GetKey(KeyCode code)
        {
            return (Input.GetKey(code) &&  (AVAILABLE_IN_BUILD || Application.isEditor));
        }
    }
}