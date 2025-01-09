using UnityEngine;

namespace GiantSword
{
    public class LayermaskAsset : ScriptableObject
    {
        [SerializeField] private LayerMask _layerMasks;

        public LayerMask layerMask => _layerMasks;
        
        // implicit operator
        public static implicit operator int(LayermaskAsset layermaskAsset)
        {
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
    }
}
