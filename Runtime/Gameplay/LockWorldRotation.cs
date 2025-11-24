using UnityEngine;

namespace JamKit
{
	public class LockWorldRotation : MonoBehaviour
	{
		[SerializeField] Vector3 _euler;
		void LateUpdate()
		{
			transform.rotation = Quaternion.Euler(_euler);
		}
	}
}