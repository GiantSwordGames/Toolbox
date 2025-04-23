namespace GiantSword
{
    using UnityEngine;


    public class IgnoreColliders : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Collider[] colliders = FindObjectsOfType<Collider>();
            foreach (var colliderA in colliders)
            {
                foreach (var colliderB in colliders)
                {
                    Physics.IgnoreCollision(colliderA, colliderB);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    

}
