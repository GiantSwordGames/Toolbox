using UnityEditor;

namespace JamKit
{
   
    [CustomPropertyDrawer(typeof(ScriptableEvent))]
    public class ScriptableEventDrawer : CreateAssetDrawer<ScriptableEvent>
    {
        protected override string customPrefix => "Event";
    }
}