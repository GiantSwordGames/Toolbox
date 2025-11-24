using UnityEngine;

namespace JamKit
{
    public class AutoDestroy : MonoBehaviour
    {
        float _duration = 1f;
        float _timer = 0f;
        
        public AutoDestroy SetDuration(float duration)
        {
            _duration = duration;
            return this;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                Destroy(gameObject);
            }
        }
        
        
    }
}