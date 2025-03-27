using UnityEngine;

namespace GiantSword
{
    public class GenerateRopeConfiguration : ScriptableObject
    {
        [SerializeField] private GameObject _segment;
        [SerializeField] private float _segmentMass = 1;
        [SerializeField] private float _segmentDrag;
        [SerializeField] private float _springStrength;
        [SerializeField] private bool _removeTheFirstJoint;

        [SerializeField]
        private float _springDamper;

        [SerializeField] private Material[] _materials;
        [SerializeField] private Vector3 _offset = Vector3.up;
        [SerializeField] private Vector3 _rotationOffset = Vector3.zero;

        public GameObject segment => _segment;

        public float segmentMass => _segmentMass;

        public float segmentDrag => _segmentDrag;

        public float springStrength => _springStrength;

        public float springDamper => _springDamper;

        public Material[] materials => _materials;

        public Vector3 offset
        {
            get => _offset;
            set => _offset = value;
        }

        public Vector3 rotationOffset => _rotationOffset;

        public bool removeTheFirstJoint => _removeTheFirstJoint;
    }
}