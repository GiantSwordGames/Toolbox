using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class Decision : MonoBehaviour
    {
        [Serializable]
        private class Option
        {
            public string name = "Option";
            public int weight =1; 
            public UnityEvent events;
        }
        [SerializeField] private SmartFloat _delay = new SmartFloat(0f);

        [SerializeField]  private List<Option> _options = new List<Option>();

        [ShowNonSerializedField] private int _lastChoice = -1;
        private List<int> _outcomes = new List<int>();

        
        
        private void Awake()
        {
            Reshuffle();            
        }

        private void Reshuffle()
        {
            _outcomes.Clear();

            for (int i = 0; i < _options.Count; i++)
            {
                for (int j = 0; j < _options[i].weight; j++)
                {
                    _outcomes.Add(i);
                }
            }

            _outcomes.Shuffle();
        }

        private Option GetNext()
        {
            _lastChoice = _outcomes[0];
            _outcomes.RemoveAt(0);
            if (_outcomes.Count == 0)
            {
                Reshuffle();
            }

            return _options[_lastChoice];
        }

        public void Trigger()
        {
            StartCoroutine(IETrigger());
        }
        
        public IEnumerator IETrigger()
        {
            if(_delay > 0)
                yield return new WaitForSeconds(_delay.value);
            
            Option choice = GetNext();
            choice.events?.Invoke();
        }



        [Button("Trigger Next")]
        private void TriggerSuccess()
        {
            Trigger();
        }
    }
}