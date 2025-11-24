using System;
using UnityEngine;

namespace JamKit
{
    public abstract class MonoFloatBase : MonoBehaviour
    {
        public abstract float value { get; set; }
        public  Action<float> onValueChangedAction { get; set; }

        public abstract float GetNormalizedValue(bool clampMinToZero = false);
    }
}