using TMPro;
using UnityEngine;

namespace GiantSword
{
    public class TransitionText : MonoBehaviour
    {
        [TextArea(2, 4)]
        [SerializeField] private string _text = "Game Over";
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        
        void OnValidate()
        {
            _textMeshPro.text = _text;
        }
    }
}
