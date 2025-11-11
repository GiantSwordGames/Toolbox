using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(LevellingCurve))]
    public class LevellingCurveDrawer : CreateAssetDrawer<LevellingCurve>
    {
        protected override string customPrefix => "Leveling";
        
    }

}