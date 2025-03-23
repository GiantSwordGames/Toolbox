using UnityEditor;

namespace GiantSword
{
   
    [CustomPropertyDrawer(typeof(ScriptableEvent))]
    public class ScriptableEventDrawer : CreateAssetDrawer<PunchAsset>
    {
        protected override string customPrefix => "Event";
    }
}