using UnityEngine;

namespace GiantSword
{
    public class EnableSelfBasedOnScriptableBool : MonoBehaviour
    {
        [SerializeField] private ScriptableBool _scriptableBool;
        void Awake()
        {
            gameObject.SetActive(_scriptableBool.value);
        }
    }
}
