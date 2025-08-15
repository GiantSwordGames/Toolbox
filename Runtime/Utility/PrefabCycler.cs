using System;
using UnityEngine;

namespace JamKit
{
    /// <summary>
    /// Add this to prefab assets you want to be cycleable.
    /// No runtime logic; used as a marker + inspector UI.
    /// </summary>
    [DisallowMultipleComponent]
    public class PrefabCycler : MonoBehaviour
    {
        // Reserved for future options if you want them (e.g., grouping).

        private void OnValidate()
        {
            if (ValidationUtility.IsPrefabAsset(this))
            {
                // transform.SetSiblingIndex(0);
            }
        }
    }
}
