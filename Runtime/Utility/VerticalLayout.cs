using System;
using UnityEngine;

namespace RichardPieterse
{
    
    public class VerticalLayout : MonoBehaviour
    {
        [SerializeField] private float _spacing = 1;

        private void OnValidate()
        {
            Apply();
        }

        void Update()
        {
            Apply();
        }

        public void Apply()
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                child.localPosition = Vector3.down *count*_spacing;
                count++;
            }
        }
    }
}
