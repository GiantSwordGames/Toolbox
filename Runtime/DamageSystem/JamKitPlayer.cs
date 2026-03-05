using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class JamKitPlayer : MonoBehaviour
    {
        private static JamKitPlayer _instance;
        [SerializeField]    private  Transform _bodyTransform;
        [FormerlySerializedAs("_health")] [SerializeField] private JamKitHealth _jamKitHealth;

        public JamKitHealth jamKitHealth => _jamKitHealth;

        public static JamKitPlayer instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = CompaitibilityHelper.FindObjectOfType<JamKitPlayer>();
                }
                return _instance;
            }
        }

        public  Transform bodyTransform => _bodyTransform != null?_bodyTransform: transform;

        
        private void Awake()
        {
            _instance = this;
        }

        public static Vector3 DirectionFrom(Vector3 position)
        {
            return (instance.bodyTransform.position - position).normalized;
        }
    }
}