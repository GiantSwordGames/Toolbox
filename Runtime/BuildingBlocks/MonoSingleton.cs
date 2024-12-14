using UnityEngine;

namespace GiantSword
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance;
        private void OnEnable()
        {
            T[] findObjectsOfType = FindObjectsOfType<T>();
            if (findObjectsOfType.Length > 1)
            {
                Destroy(gameObject);
                Debug.Log("Destroy This " + gameObject);
                return;
            }

            instance = this as T;
            instance.transform.SetParent(null);
            // DontDestroyOnLoad(instance.gameObject);
        }
    }
}