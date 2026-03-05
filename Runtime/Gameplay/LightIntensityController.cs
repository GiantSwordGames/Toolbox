using UnityEngine;

namespace JamKit
{
    public class LightIntensityController : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float _intensityLerp = 1f;
        private Light _light;
        private float _initialIntensity;

        public float intensityLerp
        {
            get => _intensityLerp;
            set
            {
                _intensityLerp = value;
            }
        }

        private void Awake()
        {
            _light = GetComponent<Light>();
            _initialIntensity = _light.intensity;
        }
        
        private void Update()
        {
            _light.intensity = _initialIntensity*_intensityLerp;
        }
        
    }
}