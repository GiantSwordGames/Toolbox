using TMPro;
using UnityEngine;

namespace JamKit
{
    public class MoneyDisplay : FindMonoBehaviourSingleton<MoneyDisplay>
    {
        [SerializeField] private VariableText _text;
        public void Set(int money)
        {
            _text.value = money;
        }
    }
}
