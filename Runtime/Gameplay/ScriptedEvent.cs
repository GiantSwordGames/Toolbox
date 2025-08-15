using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class ScriptedEvent : MonoBehaviour
    {
        [SerializeField] GameObject _enableGameObject;

        private void Start()
        {
            _enableGameObject.gameObject.SetActive(false);
        }

        [Button]
        public void Trigger()
        {
            _enableGameObject.gameObject.SetActive(true);
        }
    }
}
