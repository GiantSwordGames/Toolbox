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

        public UnityEvent mouseEnter => _mouseEnter;

        public UnityEvent mouseExit => _mouseExit;

        public UnityEvent mouseDown => _mouseDown;

        private void OnMouseEnter()
        {
            _mouseEnter?.Invoke();
        }
        
        private void OnMouseExit()
        {
            _mouseExit?.Invoke();
        }
        
        private void OnMouseDown()
        {
            _mouseDown?.Invoke();
        }

        public void AddListenersToChildren()
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.gameObject.GetComponent<Collider2D>() || child.gameObject.GetComponent<Collider>())
                {
                    MouseListener mouseListener = child.gameObject.AddComponent<MouseListener>();
                    mouseListener.mouseEnter.AddListener( _mouseEnter.Invoke);
                    mouseListener.mouseExit.AddListener( _mouseExit.Invoke);
                    mouseListener.mouseDown.AddListener( _mouseDown.Invoke);
                }
            }
        }
    }
}
