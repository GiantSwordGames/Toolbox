using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(TagAsset))]
    public class TagAssetDrawer : CreateAssetDrawer<TagAsset>
    {
        protected override string customPrefix => "Tag";
    }
}