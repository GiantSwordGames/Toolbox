using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(LayerMaskAsset))]
    public class LayerMaskAssetDrawer : CreateAssetDrawer<LayerMaskAsset>
    {
        protected override string customPrefix => "Layer";
        
    }
}