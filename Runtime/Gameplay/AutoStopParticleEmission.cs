using System.Collections;
using UnityEngine;

namespace GiantSword
{
    public class AutoStopParticleEmission : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private float _emissionDuration = 1f; // Duration for the fade out effect

        public void SetEmissionDuration(float duration)
        {
            _emissionDuration = duration;
        }

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            StartCoroutine(IEAutoDestroy());
        }

        private IEnumerator IEAutoDestroy()
        {
            yield return new WaitForSeconds(_emissionDuration);
           
            _particleSystem.SetEmissionRate(0);
            
            while (_particleSystem.particleCount != 0)
            {
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}