using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class PullStraight : MonoBehaviour
    {
        [SerializeField]  private Rigidbody _rigidbody;

        [SerializeField] private float _strength = 1;
        [SerializeField] private Vector3 _worldDirection = Vector3.up;
        [SerializeField] private Vector3 _localOffset = Vector3.up;
        

        private void FixedUpdate()
        {
            _rigidbody.AddForceAtLocalPosition(_localOffset, _worldDirection*_strength);
            _rigidbody.AddForceAtLocalPosition(-_localOffset, -_worldDirection*_strength);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 worldPosition;
            worldPosition = GetComponent<Rigidbody>().transform.TransformPoint( _localOffset);
            Debug.DrawRay(worldPosition, _worldDirection*_strength, Color.green);
            worldPosition = GetComponent<Rigidbody>().transform.TransformPoint(-_localOffset);
            Debug.DrawRay(worldPosition, -_worldDirection*_strength, Color.green);

        }
    }
}