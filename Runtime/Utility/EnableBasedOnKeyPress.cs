namespace GiantSword
{
    using UnityEngine;
    
    public class EnableBasedOnKeyPress : MonoBehaviour
    {
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private Object _enable;
        [SerializeField] private bool _onKeyDown;
    
        // Update is called once per frame
        void Update()
        {
            bool key = false;
            
            if (_onKeyDown)
            {
                if (Input.GetKeyDown(_keyCode))
                {
                    _enable.SetEnabled(true);
                }
    
            }
            else
            {
                key = Input.GetKey(_keyCode);
    
                if (key)
                {
                    _enable.SetEnabled(true);
                }
                else
                {
                    _enable.SetEnabled(false);
                }
            }
    
          
        }
    }
}
