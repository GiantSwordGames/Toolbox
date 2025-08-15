using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(ScreenShakeAsset))]
    public class ScreenShakeAssetDrawer : CreateAssetDrawer<ScreenShakeAsset>
    {
        protected override string customPrefix => "Shake";
        
    }
}