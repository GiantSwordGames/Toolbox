using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GiantSword
{
    
    [CreateAssetMenu( menuName =  MenuPaths.CREATE_ASSET_MENU +"/TextSanitizer", fileName = "TextSanitizer_")]
    public class TextSanitizer : ScriptableObject
    {
        [Space]
        [SerializeField] private RegexRule[] _regexRules;

        [TextArea(3, 10)]
        public string test;

        public string Apply(string text)
        {
            foreach (var rule in _regexRules)
            {
                text = rule.Apply(text);
            }


            return text;
        }
    }
    
    [Serializable]
    public struct RegexRule
    {
        public string description;
        public string replace;
        public string with;
        public bool disable;

        public  string Apply(string assetName)
        {
            string name = assetName;
            if ( disable == false)
                name = Regex.Replace(name,  replace, with, RegexOptions.Multiline);

            return name;
        }
    }
}