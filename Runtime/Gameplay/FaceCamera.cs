using UnityEngine;

namespace GiantSword
{
	public class FaceCamera : MonoBehaviour
	{
		void Update()
		{
			Camera cam = Camera.main;
			if (cam != null)
			{
				transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
					cam.transform.rotation * Vector3.up);
			}
		}
	}
}