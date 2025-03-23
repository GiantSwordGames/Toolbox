using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(Damage))]
    public class DamageAssetDrawer : CreateAssetDrawer<Damage>
    {
        protected override string customPrefix => "Damage";
        
    }
}