using System.Collections;
using TMPro;
using UnityEngine;

namespace JamKit
{
    public class ShowLastEffect : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _textMeshProUGUI;
        private IEnumerator Start()
        {
            _textMeshProUGUI.text = "";
            yield return new WaitForSeconds(0.01f);
            Transform activeChild = transform.GetLastActiveChild();
            if (activeChild)
            {
                _textMeshProUGUI.text = $"Juice Level: {activeChild.GetSiblingIndex()}\n{activeChild.name}";

            }
        }
    }
}