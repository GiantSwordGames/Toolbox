using UnityEngine;

namespace JamKit
{
    public abstract class DrawGizmoBase : MonoBehaviour
    {
        [SerializeField] protected Color _wireColor= Color.black.WithAlpha(.5f);
        [SerializeField] protected Color _solidColor= Color.black.WithAlpha(.5f);
        [SerializeField] protected Mode _mode = Mode.OnDrawGizmosSelected;
        protected enum Mode
        {
            OnDrawGizmosSelected,
            OnDrawGizmos,
        }

        protected virtual void Start()
        {
            
        }
        
        private  void OnDrawGizmos()
        {
            if (_mode == Mode.OnDrawGizmos && enabled)
            {
                Draw();
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_mode == Mode.OnDrawGizmosSelected && enabled)
            {
                Draw();
            }
        }

        private void Draw()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = _wireColor;
            CustomDrawWire();
            Gizmos.color = _solidColor;
            CustomDrawSolid();
        }

        protected abstract void CustomDrawWire();

        protected abstract void CustomDrawSolid();

    }
}