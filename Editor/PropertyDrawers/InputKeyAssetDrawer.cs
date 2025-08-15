using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(InputKeyAsset))]
    public class InputKeyAssetDrawer : CreateAssetDrawer<InputKeyAsset>
    {
        protected override string customPrefix => "InputKey";
        
    }
}