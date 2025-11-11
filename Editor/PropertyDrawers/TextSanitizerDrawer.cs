using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(TextSanitizer))]
    public class TextSanitizerDrawer : CreateAssetDrawer<TextSanitizer>
    {
        protected override string customPrefix => "Sanitizer";
    }
}