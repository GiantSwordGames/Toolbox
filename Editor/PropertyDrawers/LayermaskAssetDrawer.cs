using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(LayermaskAsset))]
    public class LayerMaskAssetDrawer : CreateAssetDrawer<LayermaskAsset>
    {
        protected override string customPrefix => "Layer";
        
    }
}