using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(LayermaskAsset))]
    public class LayerMaskAssetDrawer : CreateAssetDrawer<LayermaskAsset>
    {
        protected override string customPrefix => "Layer";
        
    }
    
    [CustomPropertyDrawer(typeof(LevellingCurve))]
    public class LevellingCurveDrawer : CreateAssetDrawer<LevellingCurve>
    {
        protected override string customPrefix => "Leveling";
        
    }
}