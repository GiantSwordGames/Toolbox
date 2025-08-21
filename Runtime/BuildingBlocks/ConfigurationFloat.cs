  using System;
  using UnityEngine;

namespace JamKit
{
    
    public class ConfigurationFloat : ScriptableObject
    {
        [SerializeField] private float _value;

        public float value
        {
            get => _value;
            set => _value = value;
        }

        public Action<float> onValueChanged
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public static implicit operator float(ConfigurationFloat configFloat)
        {
            return configFloat._value;
        }
    }
}
