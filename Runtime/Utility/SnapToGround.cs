using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class SnapToGround : MonoBehaviour
    {
        
        [Button]
        public void Snap()
        {
            RaycastHit hit;
            
            RuntimeEditorHelper.RecordObjectUndo(transform, "Snap");
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                transform.position = hit.point;
            }
        }
    }
}
