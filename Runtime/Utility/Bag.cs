using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public class Bag<T>
    {
        private List<T> _elements = new List<T>();
        private List<T> _shuffledElements = new List<T>();
        private bool _shuffle;

        public Bag(List<T> elements, bool shuffle = true)
        {
            _shuffle = shuffle;
            _elements = elements;
            _shuffledElements.AddRange(_elements);

            Refill();
        }
        
        public void Add(T element)
        {
            _elements.Add(element);
            Refill();
        }
        
        public void AddRange(List<T> elements)
        {
            _elements.AddRange(elements);
            Refill();
        }

        public T GetNext()
        {
            if ( _shuffledElements.Count == 0)
            {
                Refill();
            }

            T next = _shuffledElements[0];
            _shuffledElements.RemoveAt(0);
            return next;
        }

        public void Refill()
        {
            _shuffledElements.Clear();
            _shuffledElements.AddRange(_elements);
            if (_shuffle)
            {
                _shuffledElements.Shuffle();
            }
        }

        public bool IsFinished()
        {
            return _shuffledElements.Count == 0;
        }
    }
}
