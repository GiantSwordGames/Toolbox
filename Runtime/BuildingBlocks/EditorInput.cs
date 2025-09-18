using UnityEngine;

namespace JamKit
{
    public static class EditorInput
    {
        public static bool IsKeyDown(KeyCode code)
        {
            return (Input.GetKeyDown(code) && Application.isEditor);
        }

        public static bool GetKey(KeyCode code)
        {
            return (Input.GetKey(code) && Application.isEditor);
        }
    }
}