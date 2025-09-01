using UnityEngine;

namespace JamKit
{
    public class TransformProgressBar : ProgressBar
    {
        [SerializeField] private Transform _primaryBar;
        [SerializeField] private Transform _secondaryBar;



        protected override void UpdateBar(float value)
        {
            if (_primaryBar)
            {
                _primaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            }
            
            if (_secondaryBar)
            {
                _secondaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            }
        }

     
    }
}