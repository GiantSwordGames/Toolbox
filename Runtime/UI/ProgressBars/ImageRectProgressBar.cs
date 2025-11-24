using System;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public class ImageRectProgressBar : NewProgressBarBase
    {
      
        [SerializeField] private Image _primaryBar;
        
        
        protected  override void UpdateBar(float lerp)
        {
            
            if (_primaryBar)
            {
                lerp = Mathf.Clamp01(lerp);

                    _primaryBar.fillAmount = lerp;
            }
            
            // if (_secondaryBar)
            // {
            // _secondaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            // }
        }


    }
}