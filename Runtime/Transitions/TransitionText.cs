using TMPro;
using UnityEngine;

namespace JamKit
{
    public class TransitionText : MonoBehaviour
    {
        [TextArea(2, 4)]
        [SerializeField] private string _text = "Game Over";
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        public string text
        {
            get => _text;
            set
            {
                _text = value;
                _textMeshPro.text = _text;
            }
        }

        public TransitionText InstantiateAndShow(string text)
        {
            TransitionBase instantiateAndDoFullTransition = GetComponent<TransitionBase>().InstantiateAndDoFullTransition();
            var transitionText = instantiateAndDoFullTransition.GetComponent<TransitionText>();
            transitionText.text = text;
            return transitionText;
        }
        
        public void InstantiateAndShow()
        {
            TransitionBase instantiateAndDoFullTransition = GetComponent<TransitionBase>().InstantiateAndDoFullTransition();
        }
        
        void OnValidate()
        {
            _textMeshPro.text = _text;
        }
    }
}
