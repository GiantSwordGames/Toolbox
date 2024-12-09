using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class SetMaterialOnChildren : MonoBehaviour
    {
        [SerializeField] private Material _material;

        private void OnValidate()
        {
            if (gameObject.IsPrefabAsset())
            {
                return;
            }
            if (_material)
            {
                Apply();   
            }
        }

        void Start()
        {
            if (_material)
            {
                Apply();
            }
        }

        [Button]
        void Apply()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                renderer.sharedMaterial = _material;
                RuntimeEditorHelper.SetDirty(renderer);
            }
        }
    }
}
