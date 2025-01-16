using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using NaughtyAttributes;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class LevellingCurve : ScriptableObject
    {
       
        [SerializeField] private float x = 1;

        [SerializeField] private string _preProcessInput = "x*1";
        [SerializeField] private string formula = "5 + 3 * 2";
        [SerializeField] private float _roundToNearest = 0;
        [ShowNonSerializedField] float _result;
        [SerializeField] public FloatRange range = new FloatRange(0, 10);
        
        void OnValidate()
        {
            _result =  EvaluateLevel(x);
        }
        
      

        public float EvaluateLevel(float level)
        {
            float input = MathHelper.TryEvaluateExpression(_preProcessInput.Replace("x", level.ToString()), out bool _);
            float output = MathHelper.TryEvaluateExpression(formula.Replace("x", input.ToString()), out bool failed);
            
            if (_roundToNearest > 0)
            {
                output = output.RoundToNearest(_roundToNearest);
            }
            return output;
        }
    }
}
