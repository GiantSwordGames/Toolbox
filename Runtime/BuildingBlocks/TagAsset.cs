using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public class TagAsset : ScriptableObject
    {
       
    }

    public static class TagUtility
    {
        
        public static List<TagAsset> GetTags(this Component component)
        {
            TagList[] taglists = component.GetComponentsInParent<TagList>();
            List<TagAsset> _tagAssets = new List<TagAsset>();
            
            foreach (var taglist in taglists)
            {
                foreach (var tag in taglist.tags)
                {
                    _tagAssets.Add(tag);
                }
            }

            return _tagAssets;
        }

        public static bool ContainsPlayerTag( this List<TagAsset>  tagAssets)
        {
            foreach (TagAsset asset in tagAssets)
            {
                if (asset.name.Equals("Tag_Player"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
