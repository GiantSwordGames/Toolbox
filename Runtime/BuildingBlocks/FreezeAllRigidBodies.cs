using JamKit;
using UnityEngine;

public class FreezeAllRigidBodies : ActionBase
{

    protected override void TriggerInternal()
    {
        Rigidbody[] rigidbodies = CompaitibilityHelper.FindObjectsByType<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }
}