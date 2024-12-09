namespace GiantSword
{
    using UnityEngine;
    
    
    [ExecuteInEditMode]
    public class VisualizeOrientation : MonoBehaviour
    {
       [SerializeField] private Vector3 _worldUp = Vector3.up;
       [SerializeField] private Vector3 _localUp = Vector3.up;
       [SerializeField] private float _scale = 1f;
       [SerializeField] private float _length = 1f;
    
       private const float SCALE = 1;
       
       [SerializeField] private Material _worldUpArrowMaterial;
       [SerializeField] private Material _localUpArrowMaterial;
       [SerializeField] private Arrow _worldUpArrow;
       [SerializeField] private Arrow _localUpArrow;
    
       public void Update()
       {
          GizmoUtility.GetArrowGizmo(ref _worldUpArrow, this);
          GizmoUtility.GetArrowGizmo(ref _localUpArrow, this);
    
          float worldUpScale = SCALE * _scale;
          _worldUpArrow.length = worldUpScale*_length;
          _worldUpArrow.scale = worldUpScale;
          _worldUpArrow.radius = 1f;
          _worldUpArrow.direction = _worldUp;
          _worldUpArrow.position = transform.position;
          _worldUpArrow.SetMaterial(_worldUpArrowMaterial);
          _worldUpArrow.length = worldUpScale*_length*1.5f;
    
          float localUpScale = SCALE * _scale;
          _localUpArrow.scale = localUpScale;
          _localUpArrow.length = localUpScale*_length*1f;
          _localUpArrow.radius = 1.5f;
          _localUpArrow.direction = transform.TransformDirection( _localUp);
          _localUpArrow.position = transform.position ;
          _localUpArrow.SetMaterial(_localUpArrowMaterial);
    
       }
    }
}
