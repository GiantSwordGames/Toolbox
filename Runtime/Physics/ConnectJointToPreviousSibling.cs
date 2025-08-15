namespace JamKit
{
    using UnityEngine;
    
    public class ConnectJointToPreviousSibling : MonoBehaviour
    {
        void OnDrawGizmosSelected()
        {
            Joint joint = GetComponent<Joint>();
    
            if (joint && transform.GetSiblingIndex() >0)
            {
                Transform previousSibling = transform.GetPreviousSibling();
                Rigidbody rigidbody = previousSibling.GetComponent<Rigidbody>();
                if (rigidbody)
                {
                    joint.connectedBody = rigidbody;
                }
            }
        }
    }
}
