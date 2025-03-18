using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public  class SoundInstanceManager : MonoBehaviour
    {
        public static SoundInstanceManager _instance;
        public static SoundInstanceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("SoundInstanceManager").AddComponent<SoundInstanceManager>();
                    GameObject.DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }

        private List<SoundAsset> _soundsPlayedThisFrame = new List<SoundAsset>();
        private Dictionary<SoundAsset, float> _lastPlayTime = new Dictionary<SoundAsset, float>();
        
        
        public SoundInstance PlaySound(SoundAsset soundAsset, Transform parent, Vector3 position)
        {
            if (soundAsset.CanPlay() == false)
            {
                return null;
            }

            SoundInstance soundInstance = SoundInstance.Create(soundAsset, parent, position);
            _lastPlayTime[soundAsset] = Time.time;
            // _soundsPlayedThisFrame.Add(soundAsset);
            return soundInstance;   
        }

        public bool CanPlaySound(SoundAsset soundAsset)
        {
            if (soundAsset == null)
                return false;
          
            if (_lastPlayTime.ContainsKey(soundAsset))
            {
                if(Time.time - _lastPlayTime[soundAsset] >= soundAsset.cooldown) // cooldown 0 will limit to 1 play per frame
                {
                    _lastPlayTime[soundAsset] = Time.time;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void LateUpdate()
        {
            _soundsPlayedThisFrame.Clear();
        }

        public static float GetLastPlayTime(SoundAsset soundAsset)
        {
            if (Instance._lastPlayTime.ContainsKey(soundAsset))
            {
                return Instance._lastPlayTime[soundAsset];
            }
            return float.NegativeInfinity;
        }
    }
}