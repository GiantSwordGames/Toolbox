using UnityEngine;

namespace JamKit
{
    public class Player : MonoBehaviour
    {
        private static Player _instance;
        [SerializeField]    private  Transform _bodyTransform;
        [SerializeField] private Health _health;

        public Health health => _health;

        public static Player instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<Player>();
                }
                return _instance;
            }
        }

        public  Transform bodyTransform => _bodyTransform != null?_bodyTransform: transform;

        
        private void Awake()
        {
            _instance = this;
        }
    }
}