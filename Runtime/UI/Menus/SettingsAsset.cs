using UnityEngine;

namespace JamKit
{
    public class SettingsAsset : MenuOptionAsset
    {
        [SerializeField] ScriptableBool _bool; 
        [SerializeField] ScriptableFloat _float;

        public override string text
        {
            get
            {
                string str = _text +": ";
                if (_bool != null)
                {
                    str += _bool.value ? "On" : "Off";
                }
                if (_float != null)
                {
                    str += _float.value.ToString("F1");
                }

                return str;
            }
            
            set { _text = value; }
        }

        public override void Click()
        {
            base.Click();
            if (_bool != null)
            {
                _bool.value = !_bool.value;
                onTextRefreshed?.Invoke();
            }
        }
    }
}