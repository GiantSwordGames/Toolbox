using UnityEngine;

namespace GiantSword
{
public class Gib : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public new Rigidbody rigidbody => _rigidbody;

    public void Setup()
    {
        _rigidbody = gameObject.AddComponent<Rigidbody>();
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
    }
}
}