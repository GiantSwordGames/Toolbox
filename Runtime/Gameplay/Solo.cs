using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class EndOfSoloChain : MonoBehaviour
    
    {
        
    }
    public class Solo : MonoBehaviour
    {
        
        [Button("Solo Chain")]
        public void Trigger()
        {
            
            Transform t = transform;
            Solo solo = t.GetComponent<Solo>();
            while (solo != null)
            {
                if (solo.transform.parent)
                {
                    foreach (Transform child in solo.transform.parent)
                    {
                        Solo sibling = child.GetComponent<Solo>();
                        if (sibling)
                        {
                            sibling.gameObject.SetActive(solo == sibling);
                            RuntimeEditorHelper.RecordObjectUndo(sibling, "Solo");
                        }
                    }
                }

                if (solo.transform.parent)
                {
                    solo = solo.transform.parent.GetComponent<Solo>();
                }
            }
        }
    }
}
