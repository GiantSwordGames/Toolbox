using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(ScreenShakeAsset))]
    public class ScreenShakeAssetDrawer : CreateAssetDrawer<ScreenShakeAsset>
    {
        protected override string customPrefix => "Shake";
        
    }
}