using UnityEngine;

namespace GiantSword
{
    public static class LayermaskUtility 
    {
        public static int NullSafe(this LayermaskAsset layermaskAsset)
        {
            if (layermaskAsset == null)
            {
                return 1;
            }
            return layermaskAsset.value;
        }
    }
    
    public class LayermaskAsset : ScriptableObject
    {
        [SerializeField] private LayerMask _layerMasks;

        public LayerMask layerMask => _layerMasks;
        public int value => layerMask;

        // implicit operator
        public static implicit operator int(LayermaskAsset layermaskAsset)
        {
            if(layermaskAsset == null)
            {
                return 1; // Default layer mask value if null
            }
            return layermaskAsset.layerMask;
        }
        public static implicit operator bool(LayermaskAsset layermaskAsset)
        {
            return layermaskAsset != null;
        }

        public bool Contains(int layer)
        {
            return (layerMask.value & (1 << layer)) != 0 ;
        }

        public bool Contains(GameObject gameObject)
        {
            return Contains(gameObject.layer);
        }
        public bool Contains(Component component)
        {
            return Contains(component.gameObject);
        }

        public Collider2D OverlapCircle2D(Vector3 position, float raduis)
        {
            Collider2D overlapCircle = Physics2D.OverlapCircle(position, raduis, layerMask.value);
            return overlapCircle;
        }
        
        public Collider2D OverlapCircle2D(Vector3 position, float raduis, GameObject ignoreGameObject)
        {
            Collider2D[] results = Physics2D.OverlapCircleAll(position, raduis, layerMask.value);
            foreach (Collider2D collider2D in results)
            {
                if (collider2D.gameObject != ignoreGameObject)
                {
                    return collider2D;
                }
            }
            return null;
        }
        
        public Collider2D[] OverlapCircleAll2D(Vector3 position, float raduis)
        {
            return Physics2D.OverlapCircleAll(position, raduis, layerMask.value);
        }

        public void Raycast(Ray ray, out RaycastHit hit)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask.value) == false)
            {
                hit = default;
            }
        }
        
        public void SphereCast(Ray ray, float thickness, out RaycastHit hit)
        {
            if (Physics.SphereCast(ray, thickness, out hit, Mathf.Infinity, layerMask.value) == false)
            {
                hit = default;
            }
        }
    }
}
