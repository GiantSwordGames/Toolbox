using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class CoinToss : MonoBehaviour
    {
        [Min(1)]
        
        [SerializeField] private int xIn = 1;
        [SerializeField] private int everyY = 2;

        [SerializeField] private UnityEvent _onSuccess;
        [SerializeField] private UnityEvent _onFail;
        [SerializeField] private UnityEvent _onCoinTossComplete;
        

        [ShowNonSerializedField] private bool _success = false;
        private List<bool> _outcomes = new List<bool>();
        
        
        private void Awake()
        {
            Reshuffle();            
        }

        private void Reshuffle()
        {
            _outcomes.Clear();

            for (int i = 0; i < everyY; i++)
            {
                _outcomes.Add(i < xIn);
            }

            _outcomes.Shuffle();
        }

        private bool GetNext()
        {
            bool result = _outcomes[0];
            _outcomes.RemoveAt(0);
            if (_outcomes.Count == 0)
            {
                Reshuffle();
            }

            return result;
        }

        public void Trigger()
        {
            _success = GetNext();
            if(_success)
            {
                TriggerSuccess();
            }
            else
            {
                TriggerFail();
            }
            

        }

        [Button("Trigger Success")]
        private void TriggerSuccess()
        {
            _onSuccess?.Invoke();
            _onCoinTossComplete?.Invoke();
        }
        
        [Button("Trigger Failure")]
        private void TriggerFail()
        {
            _onFail?.Invoke();
            _onCoinTossComplete?.Invoke();
        }

        public static bool Flip()
        {
            return Random.value > 0.5f;
        }
    }
}
