using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace JamKit
{
    public class MuteMixer : MonoBehaviour 
    {
        [SerializeField] private ScriptableBool isNotMuted;
        [SerializeField] private string _muteParameterName = "MuteMusicVolume";
        [SerializeField] private AudioMixerGroup _musicMuteMixer;
        
        void Start()
        {
            isNotMuted.onValueChanged += UpdateMusicMute;
            UpdateMusicMute(false);
        }

        void UpdateMusicMute(bool b)
        {
            if (isNotMuted.value)
            {
                _musicMuteMixer.audioMixer.SetFloat(_muteParameterName, 0);
            }
            else
            {
                _musicMuteMixer.audioMixer.SetFloat(_muteParameterName, -80);
            }
        }

        
        [Button]
        public void Toggle()
        {
            isNotMuted.Toggle();
        }
    }
}