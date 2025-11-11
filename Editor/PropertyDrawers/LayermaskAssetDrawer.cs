using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(LayermaskAsset))]
    public class LayerMaskAssetDrawer : CreateAssetDrawer<LayermaskAsset>
    {
        protected override string customPrefix => "Layer";
        
    }
}