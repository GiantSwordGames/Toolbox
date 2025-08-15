using System;
using System.Text.RegularExpressions;

namespace JamKit.AssetRules
{
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