using Meat;
using UnityEngine;

namespace GiantSword
{
    public class AutoSetUpTile : MonoBehaviour
    {
        [SerializeField] private TileUtility _tileUtility;
        
      
        public void RefreshSelf()
        {
            Refresh(true);
        }

        private void Refresh(bool refreshAdjacent)
        {
            LayerMask layerMask = LayerMask.GetMask("Editor");
            Vector3 center = transform.position + Vector3.up;
            float offset = 2;
            EnableWall(_tileUtility.wallEast, center, Vector3.right*offset, layerMask);
            EnableWall(_tileUtility.wallWest, center, Vector3.left*offset, layerMask);
            EnableWall(_tileUtility.wallNorth, center, Vector3.forward*offset, layerMask);
            EnableWall(_tileUtility.wallSouth, center, Vector3.back*offset, layerMask);
            EnableWall(_tileUtility.floor, center, Vector3.down*offset, layerMask);
            EnableWall(_tileUtility.ceiling, center, Vector3.up*offset, layerMask);
        }

        public void RefreshAdjacent()
        {
            LayerMask layerMask = LayerMask.GetMask("Editor");
            Vector3 center = transform.position + Vector3.up;
            float offset = 2;
            RefreshAdjacent(center, offset, layerMask);
        }
        
        public void RefreshAll()
        {
            Refresh(true);
            RefreshAdjacent();
        }
        
        private void RefreshAdjacent(Vector3 center, float offset, LayerMask layerMask)
        {
            RefreshAdjacent(center, Vector3.right*offset, layerMask);
            RefreshAdjacent(center, Vector3.left*offset, layerMask);
            RefreshAdjacent( center, Vector3.forward*offset, layerMask);
            RefreshAdjacent( center, Vector3.back*offset, layerMask);
            RefreshAdjacent(center, Vector3.down*offset, layerMask);
            RefreshAdjacent( center, Vector3.up*offset, layerMask);
        }


        public bool EnableWall(GameObject wall, Vector3 origin, Vector3 offset, LayerMask layerMask)
        {
            RuntimeEditorHelper.RecordObjectUndo(wall, "EnableWall");
            Collider[] colliders = Physics.OverlapSphere(origin + offset, 0.5f, layerMask);
            
            if (colliders.Length == 0)
            {
                wall.SetActive(true);
                return true;
            }
            else
            {
                wall.SetActive(false);
                return false;
            }
        }
        
        

        public void RefreshAdjacent( Vector3 origin, Vector3 offset, LayerMask layerMask)
        {
            Collider[] colliders = Physics.OverlapSphere(origin + offset, 0.5f, layerMask);
            
            // Debug.DrawRay(origin + offset, Vector3.up, Color.magenta, 12);
            if (colliders.Length == 0)
            {
            }
            else
            {
                foreach (Collider col in colliders)
                {
                    AutoSetUpTile autoSetUpTile = col.GetComponentInParent<AutoSetUpTile>();
                    if (autoSetUpTile)
                    {
                        autoSetUpTile.Refresh(false);
                    }
                }
            }
        }
    }
}
