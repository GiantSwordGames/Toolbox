using System;
using UnityEngine;

namespace JamKit
{
    public class WaitForInput : CustomYieldInstruction
    {
        private Func<bool> _condition;

        public WaitForInput(Func<bool> condition)
        {
            _condition = condition;
        }

        public override bool keepWaiting => !_condition();
    }
}