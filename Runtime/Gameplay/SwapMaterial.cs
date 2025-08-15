using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class SwapMaterial : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private Material _material;
        [SerializeField] private Material _with;
        
        
        [Button]
        public void Trigger()
        {
            Renderer[] renderers = _root.gameObject.GetComponentsInChildren<Renderer>();
            
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == _material)
                    {
                        materials[i] = _with;
                    }
                }
                renderer.sharedMaterials = materials;
            }
        }
    }
}
