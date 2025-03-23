using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(InputKeyAsset))]
    public class InputKeyAssetDrawer : CreateAssetDrawer<InputKeyAsset>
    {
        protected override string customPrefix => "InputKey";
        
    }
}