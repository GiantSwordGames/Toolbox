using System.Collections;
using TMPro;
using UnityEngine;

namespace GiantSword
{
    public class FlashMenuOption : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;
       

        // Update is called once per frame
        public void Trigger()
        {
            StartCoroutine(IETriggerRoutine());
        }
        
        private IEnumerator IETriggerRoutine()
        {
            _textMeshPro.color= _textMeshPro.color.WithAlpha(0.5f);
            yield return new WaitForSeconds(.2f);
            _textMeshPro.color= _textMeshPro.color.WithAlpha(1f);
        }
    }
}
