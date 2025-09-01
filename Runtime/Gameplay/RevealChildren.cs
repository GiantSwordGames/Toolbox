using UnityEngine;

namespace JamKit
{
    public class RevealChildren : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float _lerp;

        public float lerp => _lerp;

        private void OnValidate()
        {
            SetFrame();
        }
        private void Awake()
        {
            SetFrame();
        }

        void SetFrame()
        {
            int frame = Mathf.FloorToInt(_lerp * (transform.childCount - 1));
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i <= frame);
            }
        }
        
        public void Decrement()
        {
            int frame = Mathf.FloorToInt(_lerp * (transform.childCount - 1));
            frame--;
            frame = Mathf.Clamp(frame, 0, transform.childCount - 1);
            _lerp = frame / (float)(transform.childCount - 1);
            _lerp = Mathf.Clamp01(_lerp);
            SetFrame();
        }
        

    }
}