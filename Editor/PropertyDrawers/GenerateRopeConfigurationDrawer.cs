using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(GenerateRopeConfiguration))]
    public class GenerateRopeConfigurationDrawer : CreateAssetDrawer<GenerateRopeConfiguration>
    {
        protected override string customPrefix => "Rope";
        
    }
}