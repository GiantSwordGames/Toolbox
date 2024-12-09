using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace GiantSword
{
    public class MenuOption : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        [SerializeField] private MenuOptionAsset _optionAsset;
        [SerializeField] private UnityEvent _onClick;
        [SerializeField] private SoundAsset _sound;
        private bool _clicked;

        private void OnValidate()
        {
            SetUp();
        }

        private void OnEnable()
        {
            _clicked = false;
        }

        private void SetUp()
        {
            if (_optionAsset)
            {
                _textMeshPro.text = _optionAsset.text;
                // _canvasGroup.alpha = _optionAsset.interactable ? 1 : 0.5f;
                _canvasGroup.interactable = _optionAsset.interactable;
            }
        }

        private void Start()
        {
            SetUp();
        }

        public void OnClicked()
        {
            if (_clicked)
            {
                return;
            }
            _sound?.Play();
            _clicked = true;
            _optionAsset.Trigger();
            _onClick.Invoke();
        }
    }
}
