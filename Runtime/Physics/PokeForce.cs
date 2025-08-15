
    using System.Collections;
    using UnityEngine;

    namespace JamKit
    {
        [ExecuteInEditMode]
        public class PokeForce : MonoBehaviour
        {
            [SerializeField] private Rigidbody _rigidbody;
            [SerializeField] private Vector3 _force = Vector3.zero;
            [SerializeField] private float _multiplier = 1f;
            [SerializeField] private bool _localSpace = false;

            [SerializeField] private LayerMask _raycastMask;

            [Space] [SerializeField] private bool _drawArrow = true;
            [SerializeField] private float _intendedForceRange = 1;
            [SerializeField] private Vector3 _arrowOffset;
            private float _lerp = 1;

            public Vector3 force
            {
                set => _force = value;
                get
                {
                    if (_localSpace)
                    {
                        return transform.TransformDirection(_force);
                    }

                    return _force;
                }
            }

            public float lerp
            {
                get => _lerp;
                set => _lerp = value;
            }


            private void LateUpdate()
            {
            }



            public void Poke()
            {
                Physics.Raycast(transform.position, force.normalized, out RaycastHit hit, Mathf.Infinity, _raycastMask);
                if (hit.collider)
                {
                    _rigidbody = hit.collider.GetComponentInParent<Rigidbody>();

                    if (_rigidbody)
                    {
                        _rigidbody.AddForceAtPosition(force * _multiplier * lerp, hit.point, ForceMode.Impulse);
                    }
                }

                StartCoroutine(IEPokeAnimation());
            }

            IEnumerator IEPokeAnimation()
            {
                Vector3 offset = Vector3.zero;

                float speed = 10;
                float lerp = 0;
                while (lerp < 1f)
                {
                    lerp += Time.deltaTime * speed;
                    Debug.Log(lerp);

                    transform.position -= offset;
                    offset = force * lerp * 0.1f;
                    transform.position += offset;
                    yield return null;
                }

                while (lerp > 0f)
                {
                    lerp -= Time.deltaTime * speed;
                    Debug.Log(lerp);

                    transform.position -= offset;
                    offset = force * lerp * 0.1f;
                    transform.position += offset;
                    yield return null;
                }
            }
        }
    }
