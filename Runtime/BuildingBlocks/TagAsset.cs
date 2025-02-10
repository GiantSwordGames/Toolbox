using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public class TagAsset : ScriptableObject
    {
       
    }

    public static class TagUtility
    {
        
        public static List<TagAsset> GetTagsOnGameObject(this Component component)
        {
            TagList[] taglists = component.GetComponents<TagList>();
            SingleTag[] singleTags = component.GetComponents<SingleTag>();
            List<TagAsset> _tagAssets = new List<TagAsset>();
            
            foreach (var taglist in taglists)
            {
                foreach (var tag in taglist.tags)
                {
                    _tagAssets.Add(tag);
                }
            }
            
            foreach (var singleTag in singleTags)
            {
                _tagAssets.Add(singleTag.tags);
            }

            return _tagAssets;
        }
        
        public static List<TagAsset> GetTagsInChildren(this Component component)
        {
            TagList[] taglists = component.GetComponentsInChildren<TagList>();
            SingleTag[] singleTags = component.GetComponentsInChildren<SingleTag>();
            List<TagAsset> _tagAssets = new List<TagAsset>();
            
            foreach (var taglist in taglists)
            {
                foreach (var tag in taglist.tags)
                {
                    _tagAssets.Add(tag);
                }
            }
            
            foreach (var singleTag in singleTags)
            {
                _tagAssets.Add(singleTag.tags);
            }

            return _tagAssets;
        }
        
        public static List<TagAsset> GetTagsInParents(this Component component)
        {
            TagList[] taglists = component.GetComponentsInParent<TagList>();
            SingleTag[] singleTags = component.GetComponentsInParent<SingleTag>();
            List<TagAsset> _tagAssets = new List<TagAsset>();
            
            foreach (var taglist in taglists)
            {
                foreach (var tag in taglist.tags)
                {
                    _tagAssets.Add(tag);
                }
            }
            
            foreach (var singleTag in singleTags)
            {
                _tagAssets.Add(singleTag.tags);
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
