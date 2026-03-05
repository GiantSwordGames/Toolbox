using System;
using UnityEngine;

namespace JamKit
{
    public class ParticleEmissionController : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float _emissionLerp = 1f;
        ParticleSystem _particleSystem;
        private float _rateOverTime;

        public float emissionLerp
        {
            get => _emissionLerp;
            set
            {
                _emissionLerp = value;
            }
        }

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _rateOverTime = _particleSystem.emission.rateOverTime.constant;
        }


        private void Update()
        {
            _particleSystem.SetEmissionRate(_emissionLerp*_rateOverTime);
            _particleSystem.SetEnabled(_emissionLerp > 0);
        }
    }
}