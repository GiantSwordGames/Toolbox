namespace JamKit
{
    // public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    // {
    //     private static T _instance;
    //
    //     public static T instance
    //     {
    //         get
    //         {
    //             if (_instance == null)
    //             {
    //                 // Try to find an existing instance in the editor.
    //                 _instance = RuntimeEditorHelper.FindAsset<T>();
    //             }
    //
    //             return _instance;
    //         }
    //     }
    //
    //     protected virtual void OnEnable()
    //     {
    //         if (_instance == null)
    //         {
    //             _instance = this as T;
    //         }
    //     }
    // }
}