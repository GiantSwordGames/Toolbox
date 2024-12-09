using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class SpawnPoint : MonoBehaviour
    {
        [Button]
        private void SetPlayerToThisPoint()
        {
            Player player = FindObjectOfType<Player>();
            RuntimeEditorHelper.RecordObjectUndo(player.transform);
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(0.1f, 0.001f, 0.1f));

            Gizmos.color = Color.cyan.WithAlpha(0.5f);
            Gizmos.DrawSphere(Vector3.zero, 1);
            Gizmos.DrawRay(Vector3.zero, Vector3.forward*1.2f);
        }
    }
}
