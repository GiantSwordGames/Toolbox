using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class MouseListener : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
    {
       

        [SerializeField] private UnityEvent _mouseEnter = new UnityEvent();
        [SerializeField] private UnityEvent _mouseExit = new UnityEvent();
        [SerializeField] private UnityEvent _mouseDown = new UnityEvent();
        [SerializeField] private UnityEvent _mouseUp = new UnityEvent();
        [SerializeField] private UnityEvent _mouseUpAnyWhere = new UnityEvent();
        [SerializeField] private bool _isMouseOver = false;

        public UnityEvent mouseEnter => _mouseEnter;

        public UnityEvent mouseExit => _mouseExit;

        public UnityEvent mouseDown => _mouseDown;
        public UnityEvent mouseUp => _mouseUp;
        public UnityEvent mouseUpAnyWhere => _mouseUpAnyWhere;

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

        private void OnMouseUp()
        {
            _mouseUp?.Invoke();
        }

        private void Start()
        {
        }


        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _mouseUpAnyWhere?.Invoke();
            }
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
                    mouseListener._mouseUp.AddListener( _mouseUp.Invoke);
                }
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseEnter();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnMouseDown();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseExit();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnMouseUp();
        }
    }
}
