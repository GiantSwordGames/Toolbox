using System.Collections;
using UnityEngine;

namespace GiantSword
{
    public class AutoDestroyParticleEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

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

            while (_particleSystem.isPlaying ==false)
            {
                yield return null;
            }

            while (_particleSystem.isStopped ==false)
            {
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
