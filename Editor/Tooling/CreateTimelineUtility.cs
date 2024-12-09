using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GiantSword
{
    public class CreateTimelineUtility
    {
        [MenuItem("CONTEXT/PlayableDirector/Create Timeline")]
        public static void CreateTimeline(MenuCommand menuCommand)
        {
            // Create a new Timeline Asset
            PlayableDirector playableDirector = menuCommand.context as PlayableDirector;
            string directoryForAssetType = GetMostCommonlyUsedDirectoryForAssetType<TimelineAsset>();

            Debug.Log("directoryForAssetType: " + directoryForAssetType);
            if (directoryForAssetType == "")
            {
                directoryForAssetType= "Assets/Project";
            }
            // Create a new Timeline Asset
            string newAssetPath = directoryForAssetType + "/Timeline_.asset";

            TimelineAsset timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
            AssetDatabase.CreateAsset(timelineAsset, newAssetPath);

            // import
            AssetDatabase.ImportAsset(newAssetPath);

            // get asset
            TimelineAsset importedTimelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(newAssetPath);
            playableDirector.playableAsset = importedTimelineAsset;

            RuntimeEditorHelper.SelectAndFocus(importedTimelineAsset);
            Debug.Log("Created " + newAssetPath, importedTimelineAsset);
        }

        public static string GetMostCommonlyUsedDirectoryForAssetType<T>() where T : Object
        {
            List<T> findAssetsOfType = RuntimeEditorHelper.FindAssetsOfType<T>();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach (var asset in findAssetsOfType)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                string directory = System.IO.Path.GetDirectoryName(assetPath);
                Debug.Log(directory);
                if (dictionary.ContainsKey(directory) == false)
                {
                    dictionary[directory] = 0;
                }
                
                dictionary[directory] += 1;
            }
            
            string mostCommonlyUsedDirectory = "";
            int highestCount = 0;
            foreach (var pair in dictionary)
            {
                if (pair.Value > highestCount)
                {
                    highestCount = pair.Value;
                    mostCommonlyUsedDirectory = pair.Key;
                }
            }

            return mostCommonlyUsedDirectory;
        }
    }
}