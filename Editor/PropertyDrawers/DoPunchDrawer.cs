using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(PunchAsset))]
    public class PunchAssetDrawer : CreateAssetDrawer<PunchAsset>
    {
        protected override string customPrefix => "Punch";
    }
}