using UnityEditor;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(Level))]
    public class LevelAssetDrawer : CreateAssetDrawer<Level>
    {
        protected override string customPrefix => "Level";
    }
}