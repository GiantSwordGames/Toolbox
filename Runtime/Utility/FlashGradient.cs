using System;
using JamKit;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JamKit
{

    public class FlashGradient : MonoBehaviour
    {
        [SerializeField] private  TimeScale _timeScale = TimeScale.Scaled;

        [SerializeField] private Graphic _graphic;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private Gradient _gradient;
        private Color _originalColor;
        private float timer = 0;

        private void OnEnable()
        {
            _originalColor = _graphic.color;
        }

        private void OnDisable()
        {
            _graphic.color = _originalColor;
        }

        private void Update()
        {
            timer += _timeScale.GetDeltaTime() *_speed;
            Color color = _gradient.Evaluate(Mathf.Repeat(timer, 1f));
            color = Color.Lerp(_originalColor, color, color.a);
            _graphic.color = color;
        }
    }
}
