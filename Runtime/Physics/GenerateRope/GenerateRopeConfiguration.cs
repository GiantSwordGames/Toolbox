using UnityEngine;

namespace GiantSword
{
    public class GenerateRopeConfiguration : ScriptableObject
    {
        [SerializeField] private GameObject _segment;
        [SerializeField] private float _segmentMass = 1;
        [SerializeField] private float _segmentDrag;
        [SerializeField] private float _springStrength;

        [SerializeField]
        private float _springDamper;

        [SerializeField] private Material[] _materials;

        public GameObject segment => _segment;

        public float segmentMass => _segmentMass;

        public float segmentDrag => _segmentDrag;

        public float springStrength => _springStrength;

        public float springDamper => _springDamper;

        public Material[] materials => _materials;
    }
}