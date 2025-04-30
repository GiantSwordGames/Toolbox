using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public class IgnoreColliders2D : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Collider2D[] colliders = FindObjectsOfType<Collider2D>();
            foreach (var colliderA in colliders)
            {
                foreach (var colliderB in colliders)
                {
                    Physics2D.IgnoreCollision(colliderA, colliderB);
                }
            }
        }
    
        
        public static void IgnoreColliders(List<Collider2D> collidersA, List<Collider2D> collidersB, bool ignore = true)
        {
            for (int i = 0; i < collidersA.Count; i++)
            {
                for (int j = 0; j < collidersB.Count; j++)
                {
                    Physics2D.IgnoreCollision(collidersA[i], collidersB[j], ignore);
                }
            }
        }
        
        
    }
}