namespace JamKit
{
    using UnityEngine;
    
    public class NeverSleep : MonoBehaviour
    {
        [SerializeField] private float _pertuibingForce = 0.0f;
        // Update is called once per frame
        void Update()
        {
            // Get rigid body and wake up
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb)
            {
                rb.WakeUp();
                if (_pertuibingForce > 0)
                {
                    var  force = Random.insideUnitSphere* _pertuibingForce;
                    var point  = transform.TransformPoint( Random.insideUnitCircle);
                    // rb.AddForceAtPosition(force, point, ForceMode.Impulse );
                    rb.AddTorque(force, ForceMode.Impulse );
                    
                    // Debug.DrawRay(point, force, Color.cyan, 0.1f);
                }
            }
            
            
        }
    }
}
