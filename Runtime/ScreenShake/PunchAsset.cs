using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    [CreateAssetMenu]
    public class PunchAsset : ScriptableObject
    {
        [SerializeField] private float _duration = 1;
         [SerializeField] private float _amplitude =1;
        [FormerlySerializedAs("_amplitude")] [SerializeField] private Vector3 _amplitudeVector = Vector3.one;
        [SerializeField] private int _oscilations = 3;

        public event Action ListenForButtonTest;

        public Vector3 amplitudeVector => _amplitudeVector*_amplitude;

        public int oscilations => _oscilations;

        public float duration => _duration;

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