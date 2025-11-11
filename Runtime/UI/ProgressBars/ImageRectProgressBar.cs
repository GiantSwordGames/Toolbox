using System;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public class ImageRectProgressBar : NewProgressBarBase
    {
        enum Mode
        {
            ApplyToSize,
            ApplyToSliceFill
        }
        [SerializeField] private Image _primaryBar;
        [SerializeField] private float _filledWidth = 100f;
        [SerializeField] private Mode _mode;
        
        
        protected  override void UpdateBar(float lerp)
        {
            
            if (_primaryBar)
            {
                lerp = Mathf.Clamp01(lerp);

                if (_mode == Mode.ApplyToSliceFill)
                {
                    _primaryBar.fillAmount = lerp;
                }
                else if (_mode==Mode.ApplyToSize)
                {
                    _primaryBar.rectTransform.sizeDelta = _primaryBar.rectTransform.sizeDelta .WithX( _filledWidth*lerp);
                }
            }
            
            // if (_secondaryBar)
            // {
            // _secondaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            // }
        }


    }
}