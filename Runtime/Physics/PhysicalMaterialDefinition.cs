using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class PhysicalMaterialDefinition : ScriptableObject
    {
        [FormerlySerializedAs("_density")] [SerializeField] private float _scienitifDensity = 1;
        [SerializeField] private float _densityMultiplierForGameFeel = 1;
        [SerializeField] private TagAsset _tag;
        [SerializeField] private PhysicsMaterial _physicMaterial;

        // [SerializeField] private SoundBank[] _knockSounds;
        // [SerializeField] private SoundBank[] _impactSounds;
        // [SerializeField] private SoundBank[] _breakSounds;
        // [SerializeField] private SoundBank[] _explodeSounds;
        
        [ShowNativeProperty] public float density => _scienitifDensity*_densityMultiplierForGameFeel;

        public PhysicsMaterial physicMaterial => _physicMaterial;

        [Button]
        private void RefreshAllInScene()
        {
            SetDensity[] findObjectsOfType = FindObjectsOfType<SetDensity>();
            foreach (var setDensity in findObjectsOfType)
            {
                setDensity.ApplyDensity();
            }
        }
    }
}