using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class DebugLogAction : MonoBehaviour
    {
        [SerializeField] private bool _throwNullReferenceException;
        [SerializeField] private string _message;
        [Button]
        public void Trigger()
        {
            Debug.Log(_message, this);
            if (_throwNullReferenceException)
            {
                throw new NullReferenceException();
            }
        }
    }
}