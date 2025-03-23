using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(PunchAsset))]
    public class DoPunchDrawer : CreateAssetDrawer<PunchAsset>
    {
        protected override string customPrefix => "Punch";
    }
}