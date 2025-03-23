using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    // [CreateAssetMenu]
    public class PunchAsset : ScriptableObject
    {
        [SerializeField] private float _delay = 0;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private float _amplitude =.2f;
        [SerializeField] private Vector3 _amplitudeVector = new Vector3(1,-1,1); 
        [SerializeField] private int _oscilations = 5;

        public event Action ListenForButtonTest;

        public Vector3 amplitudeVector => _amplitudeVector*_amplitude;

        public int oscilations => _oscilations;

        public float duration => _duration;
        public float delay => _delay;

        public float amplitude
        {
            get => _amplitude;
            set => _amplitude = value;
        }

        public PunchInstance ApplyToScale(Transform target)
        {
            return new PunchInstance(target, this, PunchInstance.Type.Scale);
        }

        [Button]
        public void Test()
        {
            ListenForButtonTest?.Invoke();
        }
    }
}