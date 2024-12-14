using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword.AssetRules
{
    public  class AssetRuleList : ScriptableSingleton<AssetRuleList>
    {
        [Serializable]
        public class IgnorePath
        {
            public string path = "";
            [FormerlySerializedAs("disabled")] [FormerlySerializedAs("enabled")] public bool disable = true;
        }
        [SerializeField] private List<IgnorePath> _globalIgnoredPaths = new List<IgnorePath>();
        [SerializeField] private List<AbstractRule> _rules;

        public List<AbstractRule> rules => _rules;

        public List<IgnorePath> globalIgnoredPaths => _globalIgnoredPaths;

        private void OnValidate()
        {
            for (int i = 0; i < _rules.Count; i++)
            {
                if (_rules[i] != null)
                {
                    _rules[i].order = i;
                }
            }
        }

    }
}