using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class CheatCodes : MonoBehaviour
    {
        
        [Serializable]
        public class Entry
        {
            [SerializeField] string _title;
            [SerializeField] bool _editorOnly;
            [SerializeField] KeyCode[] _keyCodes;
            [SerializeField] private UnityEvent _event;

            public KeyCode[] keyCodes => _keyCodes;

            public UnityEvent @event => _event;

            public bool editorOnly => _editorOnly;
        }
        
        [SerializeField] private List<Entry> _entries = new List<Entry>();
   
        
        private void Update()
        {
            foreach (Entry entry in _entries)
            {
                if(entry.editorOnly && !Application.isEditor)
                    continue;
                
                bool allKeysPressed = true;
                foreach (KeyCode keyCode in entry.keyCodes)
                {
                    if (!Input.GetKeyDown(keyCode))
                    {
                        allKeysPressed = false;
                        break;
                    }
                }

                if (allKeysPressed)
                {
                    entry.@event.Invoke();
                }
            }
        }
    }
}
