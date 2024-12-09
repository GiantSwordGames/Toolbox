using UnityEngine;

namespace GiantSword
{
    public class AngularForceOscilator : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private Vector3 _localAxis = Vector3.up;
        [SerializeField] private float _amplitude = 1;
        [SerializeField] private float _frequency = 1;
        [SerializeField] private AnimationCurve _curve;
        
        
        void FixedUpdate()
        {
            float t = Time.time * _frequency;
            float value = _amplitude * _curve.Evaluate(t);

            Vector3 axis = _localAxis;

            axis = _rigidBody.transform.TransformDirection(axis);
            Vector3 force = axis * value;
            _rigidBody.AddRelativeTorque(force);
        }
    }
}
