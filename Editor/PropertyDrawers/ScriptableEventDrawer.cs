using UnityEditor;

namespace JamKit
{
   
    [CustomPropertyDrawer(typeof(ScriptableEvent))]
    public class ScriptableEventDrawer : CreateAssetDrawer<PunchAsset>
    {
        protected override string customPrefix => "Event";
    }
}