using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class SoloChain : MonoBehaviour
    {
        [Button]
        public void Trigger()
        {
            Transform t = transform;

            while (t != null)
            {
                if (t.GetComponent<EndOfSoloChain>()) // travel up the chain until we hit a break point
                {
                    break;
                }

                int siblingIndex = t.GetSiblingIndex();

                if (t.parent != null)
                {
                    List<Transform> directChildren = t.parent.GetDirectChildren();
                    for (int i = 0; i < directChildren.Count; i++)
                    {
                        GameObject sibling = t.parent.GetChild(i).gameObject;
                        if (sibling.HasComponent<IgnoredBySoloChain>()) // dont mess with game objects that are ignored by the solo chain
                        {
                            continue;
                        }
                        else
                        {
                            sibling.gameObject.SetActive(i == siblingIndex);
                        }
                    }
                }

                t = t.parent;

            }
        }
    }
}