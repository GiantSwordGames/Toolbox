using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class DebugShortcuts : MonoBehaviour
    {
        
        [Serializable]
        public class Entry
        {
            [SerializeField] string _title;
            [SerializeField] bool _editorOnly;
            [SerializeField] bool _holdShift;
            [SerializeField] KeyCode[] _keyCodes;
            [SerializeField] private UnityEvent _event;

            public KeyCode[] keyCodes => _keyCodes;

            public UnityEvent @event => _event;

            public bool editorOnly => _editorOnly;

            public bool holdShift => _holdShift;
        }
        
        [SerializeField] private List<Entry> _entries = new List<Entry>();
   
        static DebugShortcuts _instance;

        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

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
                
                if(entry.holdShift && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                    allKeysPressed = false;

                if (allKeysPressed)
                {
                    entry.@event.Invoke();
                }
            }
        }
    }
}
