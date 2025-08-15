using UnityEngine;

namespace GiantSword
{
    public class EnableSelfBasedOnScriptableBool : MonoBehaviour
    {
        [SerializeField] private ScriptableBool _scriptableBool;
        [SerializeField] private bool _isFalse;
        void Awake()
        {

            if (_isFalse)
            {
                gameObject.SetActive(_scriptableBool.value == false);
            }
            else
            {
                gameObject.SetActive(_scriptableBool.value );
            }
        }
    }
}
