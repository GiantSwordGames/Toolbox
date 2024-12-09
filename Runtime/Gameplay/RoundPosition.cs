using UnityEngine;

namespace GiantSword
{
    public class RoundPosition : MonoBehaviour
    {
        [SerializeField] float _roundTo = 0.01f;
     

        private void OnDrawGizmosSelected()
        {
            transform.localPosition = new Vector3(
                Mathf.Round(transform.localPosition.x / _roundTo) * _roundTo,
                Mathf.Round(transform.localPosition.y / _roundTo) * _roundTo,
                Mathf.Round(transform.localPosition.z / _roundTo) * _roundTo
            ); 
        }
    }
}
