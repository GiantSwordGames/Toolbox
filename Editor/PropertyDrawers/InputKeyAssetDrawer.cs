using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(InputKeyAsset))]
    public class InputKeyAssetDrawer : CreateAssetDrawer<InputKeyAsset>
    {
        protected override string customPrefix => "InputKey";
    }
    
    [CustomPropertyDrawer(typeof(TextSanitizer))]
    public class TextSanitizerDrawer : CreateAssetDrawer<TextSanitizer>
    {
        protected override string customPrefix => "Sanitizer";
    }
}