namespace GiantSword
{
    using UnityEngine;
    
    
    [ExecuteInEditMode]
    public class MonoGizmo : MonoBehaviour
    {
        public Object createdBy;
        [SerializeField] protected float _scale = 1;
    
        public float scale
        {
            get => _scale;  
            set => _scale = value;
        }
    }
    
    public class LineGizmo : MonoGizmo
    {
        [SerializeField] private Vector3 _start;
        [SerializeField] private Vector3 _end = Vector3.up;
    
        [Space]
        [SerializeField] private Transform _shaft;
        [SerializeField] private Transform _startCap;
        [SerializeField] private Transform _endCap;
        [Space]
        
        private const float STANDARD_SCALE = 0.05f;
    
        public Vector3 start
        {
            get => _start;
            set => _start = value;
        }
    
        public Vector3 end
        {
            get => _end;
            set => _end = value;
        }
    
    
        // Update is called once per frame
        void Update()
        {
            transform.position = start.MidPoint(end);
            transform.up = start.To(end);
    
            float capRadius = STANDARD_SCALE * scale ;
            float shaftRadius = capRadius * 0.5f;
            _shaft.localScale = new Vector3(shaftRadius, start.DistanceTo(end)/2, shaftRadius);
    
            _startCap.localScale  = Vector3.one * capRadius*.9f;
            _endCap.localScale = Vector3.one * capRadius;
            _startCap.transform.position = start;
            _endCap.transform.position = end;
        }
    }
}
