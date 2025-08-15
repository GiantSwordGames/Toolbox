using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(Level))]
    public class LevelDrawer : CreateAssetDrawer<Level>
    {
        protected override string customPrefix => "Level";
    }
}