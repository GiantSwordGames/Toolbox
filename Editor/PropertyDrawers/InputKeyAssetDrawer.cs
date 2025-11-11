using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(InputAsset))]
    public class InputKeyAssetDrawer : CreateAssetDrawer<InputAsset>
    {
        protected override string customPrefix => "Input";
    }
}