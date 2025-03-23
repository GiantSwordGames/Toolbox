using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(TagAsset))]
    public class TagAssetDrawer : CreateAssetDrawer<TagAsset>
    {
        protected override string customPrefix => "Tag";
    }
}