using UnityEngine;

namespace GiantSword
{
    public static class SaveService
    {
        public static void SaveFloat( string key, float value)
        {
            PlayerPrefs.SetFloat( key, value);
        }
        
        public static float GetFloat( string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat( key, defaultValue);
        }
        
        public static void SaveBool( string key, bool value)
        {
            PlayerPrefs.SetInt( key, value?1:0);
        }
        
        public static bool GetBool( string key, bool defaultValue)
        {
            return PlayerPrefs.GetInt( key, defaultValue?1:0) == 1;
        }
        
        public static bool HasKey( string key)
        {
            return PlayerPrefs.HasKey(key);
        }


    }
}