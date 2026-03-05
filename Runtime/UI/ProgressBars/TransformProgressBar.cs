using Framework;
using UnityEngine;

namespace JamKit
{
    public class TransformProgressBar : NewProgressBarBase, IPropertyEditListener
    {
        [SerializeField] private Vector3 _axis = Vector3.forward;
        [SerializeField] private Transform _primaryBar;


        protected override void UpdateBar(float value)
        {
            if (_primaryBar)
            {
                _primaryBar.localScale = Vector3.one.Lerp(Vector3.one*_value.normalizedValue,_axis);
            }
        }

        public void OnPropertyEdited()
        {
            UpdateBar(0);
        }
    }
}