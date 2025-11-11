using UnityEngine;

public class FreezeAllRigidBodies : ActionBase
{

    protected override void TriggerInternal()
    {
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }
}