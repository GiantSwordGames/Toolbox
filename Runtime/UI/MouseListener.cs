using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class MouseListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent _mouseEnter = new UnityEvent();
        [SerializeField] private UnityEvent _mouseExit = new UnityEvent();
        [SerializeField] private UnityEvent _mouseDown = new UnityEvent();
        [SerializeField] private bool _isMouseOver = false;

        public UnityEvent mouseEnter => _mouseEnter;

        public UnityEvent mouseExit => _mouseExit;

        public UnityEvent mouseDown => _mouseDown;

        public bool isMouseOver => _isMouseOver;

        private void OnDisable()
        {
            if (_isMouseOver)
            {
                OnMouseExit();
            }
        }

        private void OnMouseEnter()
        {
            _isMouseOver = true;
            _mouseEnter?.Invoke();
        }
        
        private void OnMouseExit()
        {
            _isMouseOver = false;
            _mouseExit?.Invoke();
        }
        
        private void OnMouseDown()
        {
            _mouseDown?.Invoke();
        }

        private void Start()
        {
        }

        public void AddListenersToChildren()
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.gameObject.GetComponent<Collider2D>() || child.gameObject.GetComponent<Collider>())
                {
                    MouseListener mouseListener = child.gameObject.GetOrAddComponent<MouseListener>();
                    mouseListener.mouseEnter.AddListener( _mouseEnter.Invoke);
                    mouseListener.mouseExit.AddListener( _mouseExit.Invoke);
                    mouseListener.mouseDown.AddListener( _mouseDown.Invoke);
                }
            }
        }
    }
}
