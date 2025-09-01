using JamKit;
using TMPro;
using UnityEngine;

namespace GiantSword
{
    public class MatchNameToButtonText : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void OnDrawGizmosSelected()
        {
            TextMeshProUGUI textMeshProUGUI = GetComponentInChildren < TextMeshProUGUI > ();
            if (textMeshProUGUI)
            {
                string newName = textMeshProUGUI.text.ToUpperCamelCase();
                if (name != newName)
                {
                    name = newName;
                    RuntimeEditorHelper.SetDirty(this);
                }
            }
        }
    }
}
