using UnityEngine;
using UnityEngine.Events;

namespace RichardPieterse
{
    public class MouseListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent _mouseOver = new UnityEvent();
        [SerializeField] private UnityEvent _mouseExit = new UnityEvent();
        [SerializeField] private UnityEvent _mouseDown = new UnityEvent();

        public UnityEvent mouseOver => _mouseOver;

        public UnityEvent mouseExit => _mouseExit;

        public UnityEvent mouseDown => _mouseDown;

        private void OnMouseOver()
        {
            _mouseOver?.Invoke();
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
                    mouseListener.mouseOver.AddListener( _mouseOver.Invoke);
                    mouseListener.mouseExit.AddListener( _mouseExit.Invoke);
                    mouseListener.mouseDown.AddListener( _mouseDown.Invoke);
                }
            }
        }
    }
}
