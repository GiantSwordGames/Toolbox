using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(PunchAsset))]
    public class PunchAssetDrawer : CreateAssetDrawer<PunchAsset>
    {
        protected override string customPrefix => "Punch";
    }
}