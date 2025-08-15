using UnityEngine;

namespace JamKit
{
    public class CreateAssetButtonAttribute : PropertyAttribute
    {
        public string customPrefix;

        public CreateAssetButtonAttribute(string customPrefix = "")
        {
            this.customPrefix = customPrefix;
        }
    }
}